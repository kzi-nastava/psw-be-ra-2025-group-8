using System.Collections.Generic;
using System.Linq;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourChatRoomRepository : ITourChatRoomRepository
    {
        private readonly ToursContext _context;

        public TourChatRoomRepository(ToursContext context)
        {
            _context = context;
        }

        public TourChatRoom Create(TourChatRoom chatRoom)
        {
            _context.TourChatRooms.Add(chatRoom);
            _context.SaveChanges();
            return chatRoom;
        }

        public TourChatRoom? Get(long id)
        {
            return _context.TourChatRooms
                .Include(cr => cr.Members)
                .Include(cr => cr.Messages)
                .FirstOrDefault(cr => cr.Id == id);
        }

        public TourChatRoom? GetByTourId(long tourId)
        {
            return _context.TourChatRooms
                .Include(cr => cr.Members)
                .Include(cr => cr.Messages)
                .FirstOrDefault(cr => cr.TourId == tourId && cr.IsActive);
        }

        public List<TourChatRoom> GetByUserId(long userId)
        {
            return _context.TourChatRooms
                .Include(cr => cr.Members)
                .Include(cr => cr.Messages)
                .Where(cr => cr.Members.Any(m => m.UserId == userId && m.IsActive))
                .OrderByDescending(cr => cr.CreatedAt)
                .ToList();
        }

        public TourChatRoom Update(TourChatRoom chatRoom)
        {
            _context.TourChatRooms.Update(chatRoom);
            _context.SaveChanges();
            return chatRoom;
        }

        public void Delete(long id)
        {
            var chatRoom = _context.TourChatRooms.Find(id);
            if (chatRoom != null)
            {
                _context.TourChatRooms.Remove(chatRoom);
                _context.SaveChanges();
            }
        }

        public bool ExistsForTour(long tourId)
        {
            return _context.TourChatRooms.Any(cr => cr.TourId == tourId && cr.IsActive);
        }
    }
}
