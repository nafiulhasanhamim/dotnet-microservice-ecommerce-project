using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NotificationAPI.DTOs;
using NotificationAPI.Models;

namespace NotificationAPI.Profiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile() {
            CreateMap<Notification, NotificationReadDto>();
        }   
    }
}