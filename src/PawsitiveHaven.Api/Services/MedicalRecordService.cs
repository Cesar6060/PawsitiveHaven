using PawsitiveHaven.Api.Data.Repositories;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Services;

public class MedicalRecordService : IMedicalRecordService
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IPetRepository _petRepository;
    private readonly ILogger<MedicalRecordService> _logger;

    public MedicalRecordService(
        IMedicalRecordRepository medicalRecordRepository,
        IPetRepository petRepository,
        ILogger<MedicalRecordService> logger)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _petRepository = petRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<MedicalRecordDto>> GetPetMedicalRecordsAsync(int petId, int userId)
    {
        // Verify user owns the pet
        var pet = await _petRepository.GetByIdAsync(petId);
        if (pet == null || pet.UserId != userId)
            return Enumerable.Empty<MedicalRecordDto>();

        var records = await _medicalRecordRepository.GetByPetIdAsync(petId);
        return records.Select(MapToDto);
    }

    public async Task<MedicalRecordDto?> GetMedicalRecordByIdAsync(int id, int userId)
    {
        var record = await _medicalRecordRepository.GetByIdWithDetailsAsync(id);
        if (record == null || record.Pet.UserId != userId)
            return null;

        return MapToDto(record);
    }

    public async Task<MedicalRecordDto?> CreateMedicalRecordAsync(int petId, int userId, CreateMedicalRecordRequest request)
    {
        try
        {
            // Verify user owns the pet
            var pet = await _petRepository.GetByIdAsync(petId);
            if (pet == null || pet.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to add medical record to pet {PetId} they don't own", userId, petId);
                return null;
            }

            // Validate record type
            var validTypes = new[] { "Vaccination", "VetVisit", "Medication", "Surgery", "Other" };
            if (!validTypes.Contains(request.RecordType))
            {
                _logger.LogWarning("Invalid record type: {RecordType}", request.RecordType);
                return null;
            }

            var medicalRecord = new MedicalRecord
            {
                PetId = petId,
                RecordType = request.RecordType,
                Title = request.Title,
                Description = request.Description,
                RecordDate = request.RecordDate,
                NextDueDate = request.NextDueDate,
                Veterinarian = request.Veterinarian,
                ClinicName = request.ClinicName,
                Cost = request.Cost,
                Notes = request.Notes,
                CreatedBy = userId
            };

            await _medicalRecordRepository.AddAsync(medicalRecord);
            _logger.LogInformation("Medical record created: {Title} for pet {PetId}", medicalRecord.Title, petId);

            // Fetch with details for the response
            var createdRecord = await _medicalRecordRepository.GetByIdWithDetailsAsync(medicalRecord.Id);
            return createdRecord != null ? MapToDto(createdRecord) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating medical record for pet {PetId}", petId);
            return null;
        }
    }

    public async Task<MedicalRecordDto?> UpdateMedicalRecordAsync(int id, int petId, int userId, UpdateMedicalRecordRequest request)
    {
        try
        {
            var record = await _medicalRecordRepository.GetByIdWithDetailsAsync(id);
            if (record == null || record.PetId != petId || record.Pet.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to update medical record {RecordId} they don't have access to", userId, id);
                return null;
            }

            // Validate record type if provided
            if (request.RecordType != null)
            {
                var validTypes = new[] { "Vaccination", "VetVisit", "Medication", "Surgery", "Other" };
                if (!validTypes.Contains(request.RecordType))
                {
                    _logger.LogWarning("Invalid record type: {RecordType}", request.RecordType);
                    return null;
                }
                record.RecordType = request.RecordType;
            }

            if (request.Title != null) record.Title = request.Title;
            if (request.Description != null) record.Description = request.Description;
            if (request.RecordDate.HasValue) record.RecordDate = request.RecordDate.Value;
            if (request.NextDueDate.HasValue) record.NextDueDate = request.NextDueDate;
            if (request.Veterinarian != null) record.Veterinarian = request.Veterinarian;
            if (request.ClinicName != null) record.ClinicName = request.ClinicName;
            if (request.Cost.HasValue) record.Cost = request.Cost;
            if (request.Notes != null) record.Notes = request.Notes;

            await _medicalRecordRepository.UpdateAsync(record);
            _logger.LogInformation("Medical record updated: {RecordId} for pet {PetId}", id, petId);

            return MapToDto(record);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating medical record {RecordId}", id);
            return null;
        }
    }

    public async Task<bool> DeleteMedicalRecordAsync(int id, int petId, int userId)
    {
        try
        {
            var record = await _medicalRecordRepository.GetByIdWithDetailsAsync(id);
            if (record == null || record.PetId != petId || record.Pet.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to delete medical record {RecordId} they don't have access to", userId, id);
                return false;
            }

            await _medicalRecordRepository.DeleteAsync(record);
            _logger.LogInformation("Medical record deleted: {RecordId} for pet {PetId}", id, petId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting medical record {RecordId}", id);
            return false;
        }
    }

    public async Task<IEnumerable<UpcomingMedicalRecordDto>> GetUpcomingDueDatesAsync(int userId, int daysAhead = 30)
    {
        var records = await _medicalRecordRepository.GetUpcomingDueDatesAsync(userId, daysAhead);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return records.Select(r => new UpcomingMedicalRecordDto(
            r.Id,
            r.PetId,
            r.Pet.Name,
            r.RecordType,
            r.Title,
            r.NextDueDate!.Value,
            r.NextDueDate!.Value.DayNumber - today.DayNumber
        ));
    }

    private static MedicalRecordDto MapToDto(MedicalRecord record)
    {
        return new MedicalRecordDto(
            record.Id,
            record.PetId,
            record.Pet?.Name,
            record.RecordType,
            record.Title,
            record.Description,
            record.RecordDate,
            record.NextDueDate,
            record.Veterinarian,
            record.ClinicName,
            record.Cost,
            record.Notes,
            record.CreatedAt,
            record.CreatedBy,
            record.Creator?.Username
        );
    }
}
