using AutoMapper;
using Core.Concretes.Dtos;
using Core.Concretes.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Profiles
{
    public class AuthProfiles:Profile
    {
        public AuthProfiles()
        {
            //Register dto dan customer kaynak hedef olarak mapleme işlemi yapacağız.
            CreateMap<RegisterDto,Customer>();
        }
    }
}
