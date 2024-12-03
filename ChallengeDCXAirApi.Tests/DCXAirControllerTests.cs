namespace ChallengeDCXAirApi.Tests;

public class DCXAirControllerTests
{
    private readonly Mock<IFlightRepository> _mockFlightRepo;
    private readonly Mock<IAirportService> _mockAirportService;
    private readonly Mock<IAirportRepository> _mockAirportRepo;
    private readonly DCXAirController _controller;

    public DCXAirControllerTests()
    {
        // Crear mocks de las dependencias
        _mockFlightRepo = new Mock<IFlightRepository>();
        _mockAirportService = new Mock<IAirportService>();
        _mockAirportRepo = new Mock<IAirportRepository>();

        // Crear instancia del controlador con las dependencias mockeadas
        _controller = new DCXAirController(
            _mockFlightRepo.Object, 
            _mockAirportService.Object, 
            _mockAirportRepo.Object);
    }

    [Fact]
    public async Task GetJourney_ShouldReturnBadRequest_WhenRouteTypeIsInvalid()
    {
        // Act: Ejecutar el método GetJourney con un parámetro inválido
        var result = await _controller.GetJourney("New York", "Los Angeles", "invalidRouteType");

        // Assert: Verificar que la respuesta sea un BadRequest
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid route type. Allowed values are 'oneway' or 'roundtrip'.", actionResult.Value);
    }
}