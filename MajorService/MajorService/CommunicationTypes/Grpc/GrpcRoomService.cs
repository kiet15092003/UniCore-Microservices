using Grpc.Core;
using MajorService;
using MajorService.DataAccess;
using MajorService.Entities;
using Microsoft.EntityFrameworkCore;

namespace MajorService.CommunicationTypes.Grpc
{
    public class GrpcRoomService : GrpcRoom.GrpcRoomBase
    {
        private readonly AppDbContext _context;

        public GrpcRoomService(AppDbContext context)
        {
            _context = context;
        }

        public override async Task<RoomResponse> GetRoomById(RoomRequest request, ServerCallContext context)
        {
            var errors = new List<string>();

            // Validate GUID format
            if (!Guid.TryParse(request.Id, out Guid roomId))
            {
                errors.Add("Invalid Room ID format.");
            }

            // Query database if ID is valid
            Room? room = null;
            if (errors.Count == 0)
            {
                room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
                if (room == null)
                {
                    errors.Add("Room not found.");
                }
            }

            // Return error response if any errors exist
            if (errors.Count > 0)
            {
                return new RoomResponse
                {
                    Success = false,
                    Error = { errors }
                };
            }

            // Return success response
            return new RoomResponse
            {
                Success = true,
                Data = new RoomData
                {
                    Id = room!.Id.ToString(),
                    Name = room.Name,
                    AvailableSeats = room.AvailableSeats,
                    FloorId = room.FloorId.ToString()
                }
            };
        }
    }
}
