using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class TourChatRoomService : ITourChatRoomService
    {
        private readonly ITourChatRoomRepository _chatRoomRepository;
        private readonly IMapper _mapper;

        public TourChatRoomService(ITourChatRoomRepository chatRoomRepository, IMapper mapper)
        {
            _chatRoomRepository = chatRoomRepository;
            _mapper = mapper;
        }

        public TourChatRoomDto GetOrCreateTourChatRoom(long tourId, string tourName)
        {
            var existingRoom = _chatRoomRepository.GetByTourId(tourId);
            
            if (existingRoom != null)
                return _mapper.Map<TourChatRoomDto>(existingRoom);

            var newRoom = new TourChatRoom(tourId, tourName);
            var created = _chatRoomRepository.Create(newRoom);
            
            return _mapper.Map<TourChatRoomDto>(created);
        }

        public void AddUserToTourChat(long tourId, long userId)
        {
            var chatRoom = _chatRoomRepository.GetByTourId(tourId);
            if (chatRoom == null)
                throw new InvalidOperationException($"Tour chat room for tour #{tourId} does not exist");

            chatRoom.AddMember(userId);
            _chatRoomRepository.Update(chatRoom);
        }

        public void RemoveUserFromTourChat(long tourId, long userId)
        {
            var chatRoom = _chatRoomRepository.GetByTourId(tourId);
            if (chatRoom == null)
                return;

            try
            {
                chatRoom.RemoveMember(userId);
                _chatRoomRepository.Update(chatRoom);
            }
            catch (InvalidOperationException)
            {
                // User not a member, ignore
            }
        }

        public List<TourChatRoomDto> GetUserChatRooms(long userId)
        {
            var chatRooms = _chatRoomRepository.GetByUserId(userId);
            return _mapper.Map<List<TourChatRoomDto>>(chatRooms);
        }

        public void AddMessage(long chatRoomId, long senderId, string content)
        {
            var chatRoom = _chatRoomRepository.Get(chatRoomId);
            if (chatRoom == null)
                throw new KeyNotFoundException($"Chat room with ID {chatRoomId} not found");

            chatRoom.AddMessage(senderId, content);
            _chatRoomRepository.Update(chatRoom);
        }

        public List<TourChatMessageDto> GetMessages(long chatRoomId, long userId)
        {
            var chatRoom = _chatRoomRepository.Get(chatRoomId);
            if (chatRoom == null)
                throw new KeyNotFoundException($"Chat room with ID {chatRoomId} not found");

            if (!chatRoom.IsMember(userId))
                throw new UnauthorizedAccessException("You are not a member of this chat room");

            return _mapper.Map<List<TourChatMessageDto>>(chatRoom.Messages.OrderBy(m => m.SentAt).ToList());
        }
    }
}
