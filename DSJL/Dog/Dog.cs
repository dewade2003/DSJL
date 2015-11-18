using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperDog;
using System.Xml.Linq;

namespace DSJL.Dog
{
    class Dog
    {
        const string vendorCode = "M1iorSG2B69nQrnyt9P5mnjTFAQUU1GTvRiAcnkRBhyHCqlLZFPpAhMpY+XHkTQUXbjgUkBIBEB0duP4jONJJ1YYJkqgB5Y8FO6HS5fa1eqoLrjO/3coYFLHNUIYqksY5aAmnrCsUsnm+bP2l60vcY79yUVhRyEniw57LhucFMQwm8qRSll+VyqolGuVuUIM9KjPGkxX9GPOzZip1cgvsRuLS+74HBt1qaP+Spthq5J+uu92Otehb0ngyi9BW1Dbph4TUwBE57VOw6jpDHq5uwq2KrE0ZJCp81Pv+bYRoS9KNbzhNsBXYwcjXBp8CKjo4PtLaNXD7vlgpqfuojOiFTnXENmofrzK4jROMuZrFw4BrGoXg5EEjHcF4KiSwUfHqQ1wrZJhpesRbQWwVYhszCdhSRvJJeDKNBYK77cdABlKvIwrv8RNQnEqDE3HqF7yEIlADOafW21bdIkjjdAe2yGZb9Q6Q3lCttICg7Zkb5dy+SJbY3vWIUjhpbuYw9qBe+KVCa+t545Y4/NfCWfbasYACwKvjxEtV+7RBFlJh16Ew2GPIrpBjWUCneJdXxUPerrWL85mjQAKgvx5wbYdPXtAKPSiMoCYM7ScnRtwoSVxOxNEaLp98zF8mucFG9QdZNyIOyIwIvM+na62uOplccpUxbv+widnqJh2xeV2y7PuzEREEAPbhPKrlxTuS8S9rf27c2P0ESIt4kp/yUJROdj5Wq4kYKnWzxD4din1dgwIe6YaCSauEJv0+4aadYxQW4j5s8hL4mr6eqwQfyzovrlz+rQL8ZRlnfulUMb7hzxAJ/I/9jeNvte2NLjpe6y34X6JX5tl7j/6zipes/pNjbUoBlrIJq9/McVH9iWPENGVU9/s51p820GT0h/zZw0a3KonCub3tMhX5ja9U4UT+4vYvtpwZ0QDLV+hMUtaAMDdEQ1o4r7x6UJZ3PmUK071FjmSy/mFkqoAc0FqeWpzTw==";

        static SuperDog.Dog dog = new SuperDog.Dog();
        /// <summary>
        /// 检查狗
        /// </summary>
        public static bool CheckDog(out string errorString)
        {
            bool checkResult = false;
            errorString = "";
            
            dog = new SuperDog.Dog(new DogFeature(100));
            DogStatus ds = dog.Login(vendorCode);
            switch (ds) { 
                case DogStatus.StatusOk:
                    checkResult = true;
                    break;
                case DogStatus.LocalCommErr:
                    errorString = "请安装加密狗驱动！";
                    break;
                case DogStatus.TimeError:
                    errorString = "系统时钟已被篡改，请修改为当前时间！";
                    break;
                case DogStatus.DogNotFound:
                    errorString = "未找到所需的加密狗!";
                    break;
                case DogStatus.FeatureExpired:
                    errorString = "许可证已到期！";
                    break;
                case DogStatus.DeviceError:
                    errorString = "与超级狗通讯时出现USB 通信错误，请将加密狗插入其他USB接口重试！";
                    break;
            }
            dog.Logout();
            return checkResult;
        }

        /// <summary>
        ///
        /// </summary>
        public static void LogoutDog()
        {
            dog.Logout();
        }
      
    }
}
