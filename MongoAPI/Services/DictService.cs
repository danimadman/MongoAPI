using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using MongoAPI.Models.Dto;
using MongoAPI.Models.Enums;

namespace MongoAPI.Services
{
    public class DictService
    {
        public DictService() { }

        public List<ComfortLevelDto> GetDictComfortLevel() => 
            Enum.GetValues<ComformLevelEnum>().Select(x => new ComfortLevelDto()
            {
                Id = (int)x,
                Name = GetDescription(x)
            }).ToList();
        
        private static string GetDescription(Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =Attribute.GetCustomAttribute(field,typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                        return attr.Description;
                }
            }
            return null;
        }
    }
}