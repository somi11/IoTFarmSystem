using IoTFarmSystem.DeviceManagement.Application.Contracts.Repositories;
using IoTFarmSystem.DeviceManagement.Domain.Aggregates;
using IoTFarmSystem.SharedKernel.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IoTFarmSystem.DeviceManagement.Application.Commands.CreateDevice
{
    public class CreateDeviceCommandHandler : IRequestHandler<CreateDeviceCommand, Result<Guid>>
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateDeviceCommandHandler> _logger;

        public CreateDeviceCommandHandler(
            IDeviceRepository deviceRepository,
            IUnitOfWork unitOfWork,
            ILogger<CreateDeviceCommandHandler> logger)
        {
            _deviceRepository = deviceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(CreateDeviceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating device with SerialNumber: {SerialNumber}", request.SerialNumber);

            // 1. Ensure uniqueness of SerialNumber
            if (await _deviceRepository.SerialNumberExistsAsync(request.SerialNumber))
            {
                _logger.LogWarning("Device with SerialNumber {SerialNumber} already exists", request.SerialNumber);
                return Result<Guid>.Fail($"Device with serial number '{request.SerialNumber}' already exists.");
            }

            // 2. Create aggregate root
            var device = new Device(
                Guid.NewGuid(),
                request.DeviceName,
                request.SerialNumber,
                request.TenantId,
                request.AzureIotDeviceId,
                request.ConnectionString,
                request.DeviceType,
                request.MaxIntensity,
                request.PowerRatingWatts,
                request.LightSpectrum,
                request.Manufacturer,
                request.ModelNumber,
                request.FirmwareVersion,
                request.Location,
                request.Description
            );

            // 3. Persist
            await _deviceRepository.AddAsync(device);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Device {DeviceId} created successfully", device.Id);

            return Result<Guid>.Success(device.Id);
        }
    }
}
