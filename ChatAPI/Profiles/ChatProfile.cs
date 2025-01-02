using AutoMapper;
using ChatAPI.DTO;
using ChatAPI.DTOs;

namespace ChatAPI.Profiles
{
    public class ChatProfile : Profile
    {
        
        public ChatProfile()
        {
            CreateMap<Conversation, ConversationDTO>();

            CreateMap<CreateConversationDTO, Conversation>();

            CreateMap<Message, MessageDTO>();
            
            CreateMap<CreateMessageDTO, Message>();

            CreateMap<Conversation, UnreadMessageCountDTO>();

        }

    }
}
