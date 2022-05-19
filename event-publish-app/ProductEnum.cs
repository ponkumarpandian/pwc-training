using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPublish
{
    public enum ProductEnum
    {
        iPhone12 = 1,
        Oneplus10G = 2,
        RedMeNotePro = 3,
        RedMeK20Pro = 4,
        OnePlusNordCE2 = 5,
        SamsungGalaxyM535G = 6
    }

    public static class Constants
    {
        public static List<Tuple<ProductEnum, int>> ProductPriceMap()
        {
            return new List<Tuple<ProductEnum, int>>() {
                new Tuple<ProductEnum, int>( ProductEnum.iPhone12, 65000),
                new Tuple<ProductEnum, int>( ProductEnum.RedMeNotePro, 23000),
                new Tuple<ProductEnum, int>( ProductEnum.RedMeK20Pro, 29000),
                new Tuple<ProductEnum, int>( ProductEnum.Oneplus10G, 55000),
                new Tuple<ProductEnum, int>( ProductEnum.SamsungGalaxyM535G, 27000),
                new Tuple<ProductEnum, int>( ProductEnum.OnePlusNordCE2, 37000),
            };
        }
    }
}
