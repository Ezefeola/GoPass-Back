using Moq;
using Microsoft.AspNetCore.Mvc;
using GoPass.API.Controllers;
using GoPass.Application.Services.Interfaces;
using GoPass.Domain.DTOs.Request.PaginationDTOs;
using GoPass.Domain.Models;
using GoPass.Domain.DTOs.Request.ReventaRequestDTOs;
using GoPass.Domain.DTOs.Response.ReventaResponseDTOs;
using Microsoft.AspNetCore.Http;

namespace GoPass.Test.ControllerTest
{
    namespace GoPass.Tests
    {
        public class ReventaControllerTests
        {
            private readonly Mock<IReventaService> _reventaService;
            private readonly Mock<IUsuarioService> _usuarioService;
            private readonly Mock<IEntradaService> _entradaServiceMock;
            private readonly Mock<IGopassHttpClientService> _gopassHttpClientServiceMock;

            private readonly ReventaController _controller;
            private List<Reventa> _resalesList;
            private PaginationDto _paginationDto;

            public ReventaControllerTests()
            {
                // Initialize the mocks
                _reventaService = new Mock<IReventaService>();
                _usuarioService = new Mock<IUsuarioService>();
                _entradaServiceMock = new Mock<IEntradaService>();
                _gopassHttpClientServiceMock = new Mock<IGopassHttpClientService>();

                // Initialize the controller with mocked dependencies
                _controller = new ReventaController(
                    _reventaService.Object,
                    _usuarioService.Object,
                    _entradaServiceMock.Object,
                    _gopassHttpClientServiceMock.Object);

                // Mock del HttpContext para añadir los headers
                var mockHttpContext = new Mock<HttpContext>();
                var mockRequest = new Mock<HttpRequest>();

                mockRequest.Setup(r => r.Headers)
                           .Returns(new HeaderDictionary(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                           {
                   { "Authorization", "Bearer test-token" }
                           }));

                mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                };

                //Values to Test
                this.InitValues();
            }

            [Fact]
            public async Task GetResales_WithList()
            {
                this._reventaService.Setup(service => service.GetAllWithPaginationAsync(_paginationDto))
                    .ReturnsAsync(_resalesList);

                //Act
                IActionResult result = await _controller.GetResales(_paginationDto);

                //Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var returnValue = Assert.IsType<List<Reventa>>(okResult.Value);
                List<Reventa>? data = okResult.Value as List<Reventa>;

                Assert.True(data?.Count() <= _paginationDto.PageSize);

                var paged = _resalesList.Skip((_paginationDto.PageNumber - 1) * _paginationDto.PageSize).Take(_paginationDto.PageSize);

                Assert.True(paged.Any(x => x.GetHashCode() == returnValue.First().GetHashCode()));
            }

            [Fact]
            public async Task GetResales_Empty()
            {
                var emptyResalesList = new List<Reventa>();

                this._reventaService
                    .Setup(service => service.GetAllWithPaginationAsync(_paginationDto))
                    .ReturnsAsync(emptyResalesList);

                var result = await _controller.GetResales(_paginationDto);

                var okResult = Assert.IsType<OkObjectResult>(result);
                var returnValue = Assert.IsType<List<Reventa>>(okResult.Value);
                Assert.Empty(returnValue);
            }

            [Fact]
            public async Task BuyTicket_UserBuyingOwnTicket_ReturnsBadRequest()
            {
                var buyEntradaRequestDto = new BuyEntradaRequestDto { EntradaId = 1 };
                var resale = new Reventa { EntradaId = 1, VendedorId = 123 };

                _usuarioService
                    .Setup(service => service.GetUserIdByTokenAsync(It.IsAny<string>()))
                    .ReturnsAsync("123"); 

                _reventaService
                    .Setup(service => service.GetResaleByEntradaIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(resale);

                var result = await _controller.BuyTicket(buyEntradaRequestDto);

                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal("Esta intentando comprar su propia entrada, lo cual no tiene sentido", badRequestResult.Value);
            }

            [Fact]
            public async Task BuyTicket_ValidUser_ReturnsOk()
            {
                var buyEntradaRequestDto = new BuyEntradaRequestDto { EntradaId = 1 };
                var resale = new Reventa { Id = 1, EntradaId = 1, VendedorId = 123 };

                _usuarioService
                    .Setup(service => service.GetUserIdByTokenAsync(It.IsAny<string>()))
                    .ReturnsAsync("456");

                _reventaService
                    .Setup(service => service.GetResaleByEntradaIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(resale);

                _reventaService
                    .Setup(service => service.Update(It.IsAny<int>(), It.IsAny<Reventa>()))
                    .ReturnsAsync(resale);

                var result = await _controller.BuyTicket(buyEntradaRequestDto);

                var okResult = Assert.IsType<OkObjectResult>(result);
                var responseDto = Assert.IsType<ReventaResponseDto>(okResult.Value);
                Assert.Equal(resale.EntradaId, responseDto.EntradaId);
                Assert.Equal("456", responseDto.CompradorId.ToString());
            }

            [Fact]
            public async Task BuyTicket_InvalidToken_ReturnsException()
            {             
                var buyEntradaRequestDto = new BuyEntradaRequestDto { EntradaId = 1 };

                _usuarioService
                    .Setup(service => service.GetUserIdByTokenAsync(It.IsAny<string>()))
                    .ThrowsAsync(new Exception("Token nulo o vacío."));

                await Assert.ThrowsAsync<Exception>(() => _controller.BuyTicket(buyEntradaRequestDto));
            }

            [Fact]
            public async Task PublishResaleTicket_ValidUserAndTicket_ReturnsOk()
            {
                var publishReventaRequestDto = new PublishReventaRequestDto
                {
                    CodigoQR = "valid-qr-code",
                    Precio = 1000m,
                    ResaleDetail = "Detalles de la reventa"
                };

                var userId = 123;
                var authorizationHeader = "Bearer test-token";

                var usuario = new Usuario
                {
                    Id = userId,
                    Nombre = "Test User",
                    DNI = "12345678",
                    NumeroTelefono = "123456789",
                    VerificadoEmail = true,
                    VerificadoSms = true
                };

                var entrada = new Entrada
                {
                    Id = 1,
                    CodigoQR = "valid-qr-code",
                };

                var entradaToCreate = new Entrada
                {
                    Id = 2,
                    CodigoQR = "valid-qr-code",
                };

                var reventa = new Reventa
                {
                    EntradaId = entradaToCreate.Id,
                    Precio = 1000m,
                    ResaleDetail = "Detalles de la reventa"
                };

                var expectedPublishReventaRequestDto = new PublishReventaRequestDto
                {
                    Precio = reventa.Precio,
                    ResaleDetail = reventa.ResaleDetail
                };

                _usuarioService
                    .Setup(s => s.GetUserIdByTokenAsync(It.IsAny<string>()))
                    .ReturnsAsync(userId.ToString());

                _usuarioService
                    .Setup(s => s.ValidateUserCredentialsToPublishTicket(It.IsAny<int>()))
                    .ReturnsAsync(true);

                _gopassHttpClientServiceMock
                    .Setup(s => s.GetTicketByQrAsync(It.IsAny<string>()))
                    .ReturnsAsync(entrada);

                _entradaServiceMock
                    .Setup(s => s.PublishTicket(It.IsAny<PublishEntradaRequestDto>(), It.IsAny<int>()))
                    .ReturnsAsync(entradaToCreate);

                _reventaService
                    .Setup(s => s.PublishTicketAsync(It.IsAny<Reventa>(), It.IsAny<int>()))
                    .ReturnsAsync(reventa);

                var result = await _controller.PublishResaleTicket(publishReventaRequestDto);

                var okResult = Assert.IsType<OkObjectResult>(result);
                var resultValue = Assert.IsType<PublishReventaRequestDto>(okResult.Value);

                Assert.Equal(expectedPublishReventaRequestDto.Precio, resultValue.Precio);
                Assert.Equal(expectedPublishReventaRequestDto.ResaleDetail, resultValue.ResaleDetail);

                _usuarioService.Verify(s => s.GetUserIdByTokenAsync(authorizationHeader), Times.Once);
                _usuarioService.Verify(s => s.ValidateUserCredentialsToPublishTicket(userId), Times.Once);
                _gopassHttpClientServiceMock.Verify(s => s.GetTicketByQrAsync(publishReventaRequestDto.CodigoQR), Times.Once);
                _entradaServiceMock.Verify(s => s.PublishTicket(It.IsAny<PublishEntradaRequestDto>(), userId), Times.Once);
                _reventaService.Verify(s => s.PublishTicketAsync(It.IsAny<Reventa>(), userId), Times.Once);
            }


            [Fact]
            public async Task PublishResaleTicket_InvalidUserCredentials_ReturnsBadRequest()
            {
                var publishReventaRequestDto = new PublishReventaRequestDto
                {
                    CodigoQR = "test-qr",
                };
                var userId = 123;

                _usuarioService
                    .Setup(service => service.GetUserIdByTokenAsync(It.IsAny<string>()))
                    .ReturnsAsync(userId.ToString());

                _usuarioService
                    .Setup(service => service.ValidateUserCredentialsToPublishTicket(It.IsAny<int>()))
                    .ReturnsAsync(false);

                var result = await _controller.PublishResaleTicket(publishReventaRequestDto);

                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal("Debe tener todas sus credenciales en regla para poder publicar una entrada", badRequestResult.Value);
            }

            [Fact]
            public async Task PublishResaleTicket_TicketNotFound_ReturnsBadRequest()
            {
                var publishReventaRequestDto = new PublishReventaRequestDto
                {
                    CodigoQR = "invalid-qr",
                };
                var userId = 123;

                _usuarioService
                    .Setup(service => service.GetUserIdByTokenAsync(It.IsAny<string>()))
                    .ReturnsAsync(userId.ToString());

                _usuarioService
                    .Setup(service => service.ValidateUserCredentialsToPublishTicket(It.IsAny<int>()))
                    .ReturnsAsync(true);

                _gopassHttpClientServiceMock
                    .Setup(service => service.GetTicketByQrAsync(It.IsAny<string>()))
                    .ThrowsAsync(new Exception("Ticket no encontrado"));

                var result = await _controller.PublishResaleTicket(publishReventaRequestDto);

                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal("Ticket no encontrado", badRequestResult.Value);
            }

            private void InitValues()
            {
                _paginationDto = new PaginationDto { PageNumber = 1, PageSize = 10 };

                _resalesList = new List<Reventa>
            {
                new Reventa { Id = 1, EntradaId = 1, VendedorId = 1, FechaReventa = DateTime.Now, ResaleDetail = "Random 1" , CompradorId = 1,Precio = decimal.One },
                new Reventa { Id = 2, EntradaId = 2, VendedorId = 2, FechaReventa = DateTime.Now, ResaleDetail = "Random 2" , CompradorId = 2,Precio = decimal.One * 2 },
                new Reventa { Id = 3, EntradaId = 3, VendedorId = 3, FechaReventa = DateTime.Now, ResaleDetail = "Random 3" , CompradorId = 3,Precio = decimal.One * 3 },
                new Reventa { Id = 4, EntradaId = 4, VendedorId = 4, FechaReventa = DateTime.Now, ResaleDetail = "Random 4" , CompradorId = 4,Precio = decimal.One * 4 },
                new Reventa { Id = 5, EntradaId = 5, VendedorId = 5, FechaReventa = DateTime.Now, ResaleDetail = "Random 5" , CompradorId = 5,Precio = decimal.One * 5 },
            };
            }
        }
    }
}
