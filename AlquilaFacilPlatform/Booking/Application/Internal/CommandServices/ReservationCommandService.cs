using AlquilaFacilPlatform.Booking.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Booking.Domain.Model.Commands;
using AlquilaFacilPlatform.Booking.Domain.Repositories;
using AlquilaFacilPlatform.Booking.Domain.Services;
using AlquilaFacilPlatform.Shared.Application.Internal.OutboundServices;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.Booking.Application.Internal.CommandServices;

public class ReservationCommandService(
 IUserExternalService userExternalService,
 ILocalExternalService localExternalService, 
 INotificationExternalService notificationExternalService,
 IReservationRepository reservationRepository,
 IUnitOfWork unitOfWork) : IReservationCommandService
{
 public async Task<Reservation> Handle(CreateReservationCommand command)
 {
     if (!userExternalService.UserExists(command.UserId))
     {
         throw new Exception("There are no users matching the id specified");
     }

     var localExists = await localExternalService.LocalExists(command.LocalId);
     if (!localExists)
     {
         throw new Exception("Local does not exist");
     }
     if(command.StartDate > command.EndDate)
     {
         throw new Exception("Start date must be less than end date");
     }
     if (command.StartDate.Year < DateTime.Now.Year || command.StartDate.Month < DateTime.Now.Month || command.StartDate.Day < DateTime.Now.Day)
     {
         throw new Exception("Start date must be greater than current date");
     }
     if (command.EndDate.Year < DateTime.Now.Year || command.EndDate.Month < DateTime.Now.Month || command.EndDate.Day < DateTime.Now.Day)
     {
         throw new Exception("End date must be greater than current date");
     }

     if (await localExternalService.IsLocalOwner(command.UserId, command.LocalId))
     {
            throw new BadHttpRequestException("User is the owner of the local, he cannot make a reservation");
     }

     // Check for overlapping reservations
     var hasOverlap = await reservationRepository.HasOverlappingReservationAsync(
         command.LocalId,
         command.StartDate,
         command.EndDate
     );

     if (hasOverlap)
     {
         throw new BadHttpRequestException("Ya existe una reserva para este local en las fechas seleccionadas. Por favor, elige otras fechas.");
     }

     var reservationCreated = new Reservation(command);
     await reservationRepository.AddAsync(reservationCreated);
     await unitOfWork.CompleteAsync();
     
     var ownerId = await localExternalService.GetOwnerIdByLocalId(command.LocalId);
     await notificationExternalService.CreateNotification(
         "Nueva reservación",
         $"Tienes una nueva reservación desde el {command.StartDate:dd/MM/yyyy} a las {command.StartDate:HH:mm} hasta el {command.EndDate:dd/MM/yyyy} a las {command.EndDate:HH:mm}. Verifica los datos del voucher de pago para corroborar que el depósito se realizó correctamente",
         ownerId
     );
     return reservationCreated;
 }

 public async Task<Reservation> Handle(UpdateReservationDateCommand reservation)
 {
     if(reservation.StartDate > reservation.EndDate)
     {
         throw new Exception("Start date must be less than end date");
     }
     if (reservation.StartDate.Year < DateTime.Now.Year || reservation.StartDate.Month < DateTime.Now.Month || reservation.StartDate.Day < DateTime.Now.Day)
     {
            throw new Exception("Start date must be greater than current date");
     }
     if (reservation.EndDate < DateTime.Now)
     {
         throw new Exception("End date must be greater than current date");
     }

     var reservationToUpdate = await reservationRepository.FindByIdAsync(reservation.Id);
        if (reservationToUpdate == null)
        {
            throw new Exception("Reservation does not exist");
        }

        // Check for overlapping reservations (excluding the current reservation)
        var hasOverlap = await reservationRepository.HasOverlappingReservationAsync(
            reservationToUpdate.LocalId,
            reservation.StartDate,
            reservation.EndDate,
            reservation.Id
        );

        if (hasOverlap)
        {
            throw new BadHttpRequestException("Ya existe una reserva para este local en las fechas seleccionadas. Por favor, elige otras fechas.");
        }

        reservationToUpdate.UpdateDate(reservation);
        reservationRepository.Update(reservationToUpdate);
        await unitOfWork.CompleteAsync();
        return reservationToUpdate;
 }

 public async Task<Reservation> Handle(DeleteReservationCommand reservation)
 {
     var reservationToDelete = await reservationRepository.FindByIdAsync(reservation.Id);
     if (reservationToDelete == null)
     {
         throw new Exception("Reservation does not exist");
     }

     reservationRepository.Remove(reservationToDelete);
     await unitOfWork.CompleteAsync();
     return reservationToDelete;
 }
}