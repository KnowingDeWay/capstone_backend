using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ext_Dynamics_API_Tests.HelperModules
{
    public class CanvasTokenHelper
    {
        private readonly Random _random;

        public CanvasTokenHelper()
        {
            _random = new Random();
        }

        public string CanvasTokenGenerator(int length = 70)
        {
            var fakeToken = "";
            for(int i = 0; i < length; i++)
            {
                var ch = _random.Next(32, 127);
                fakeToken = fakeToken + $"{ch}";
            }
            return fakeToken;
        } 
    }
}
