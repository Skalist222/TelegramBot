//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Text.Json;

//namespace BlaBlaInfoBot
//{
//    internal class JsonWorker
//    {
//        public string Serialize(object o,Type t)
//        {
//            return JsonSerializer.Serialize(o,t);
//        }
//        public bool SaveInFile(string pathFile, object o, Type t)
//        {
//            try
//            {
//                File.AppendAllText(pathFile, Serialize(o, t));
//            }
//            catch
//            {
//                Console.WriteLine("Ошибка сохранение пользователей в файл");
//                return false;
//            }
            
//        }
//    }
//}
