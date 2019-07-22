using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace ConsolePet
{
    public class Pet
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    /// <summary>
    /// POST requests:
    /// Adds pets to pet repository. Returns created pet as JSON object.
    /// http://localhost:4141/pets?name=Meow&type=Cat
    /// http://localhost:4141/pets?name=Woof&type=Dog
    /// GET requests:
    /// Returns current pets repository as JSON object
    /// http://localhost:4141/pets
    /// </summary>

    class Program
    {
        private static string Hello = "Hello World!";
        private static List<Pet> PetRepo = new List<Pet>();
        private static string EmptyRepo = "No pets found...";

        #region async version
        static void Main(string[] args)
        {
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://localhost:4141/");
                listener.Start();

                Console.WriteLine("Server activated, awaiting for requests now...");
                Console.WriteLine();

                while (true)
                {
                    PetProcessing(listener);
                }
            }
        }

        static void PetProcessing(HttpListener listener)
        {
            var result = listener.BeginGetContext(PetCallback, listener);
            result.AsyncWaitHandle.WaitOne();
        }

        public static void PetCallback(IAsyncResult result)
        {
            var listener = (HttpListener)result.AsyncState;
            var context = listener.EndGetContext(result);
            var request = context.Request;
            var response = context.Response;

            var serializer = new JavaScriptSerializer();

            if (request.HttpMethod == "GET")
            {
                if (request.RawUrl.ToLower() == "/pets")
                {
                    if (PetRepo.Count > 0)
                    {
                        Console.WriteLine(serializer.Serialize(PetRepo));
                        HttpOutput(Encoding.UTF8.GetBytes(serializer.Serialize(PetRepo)), response);
                    }
                    else
                    {
                        Console.WriteLine(EmptyRepo);
                        HttpOutput(Encoding.UTF8.GetBytes(EmptyRepo), response);
                    }
                }
                else
                {
                    Console.WriteLine(Hello);
                    HttpOutput(Encoding.UTF8.GetBytes(Hello), response);
                }
            }
            else if (request.HttpMethod == "POST")
            {
                var obj = new Pet()
                {
                    Name = request.QueryString[request.QueryString.GetKey(0)],
                    Type = request.QueryString[request.QueryString.GetKey(1)],
                };

                PetRepo.Add(obj);

                Console.WriteLine(serializer.Serialize(obj));
                HttpOutput(Encoding.UTF8.GetBytes(serializer.Serialize(obj)), response);
            }
        }
        #endregion

        #region sync version
        //static void Main(string[] args)
        //{
        //    var repo = new List<Pet>();

        //    using (var listener = new HttpListener())
        //    {
        //        listener.Prefixes.Add("http://localhost:4141/");
        //        listener.Start();

        //        Console.WriteLine("Server activated, awaiting for requests now...");
        //        Console.WriteLine();

        //        while (true)
        //        {
        //            var ctx = listener.GetContext();
        //            var request = ctx.Request;
        //            var response = ctx.Response;

        //            var serializer = new JavaScriptSerializer();

        //            if (request.HttpMethod == "GET")
        //            {
        //                if (request.RawUrl.ToLower() == "/pets")
        //                {
        //                    Console.WriteLine(serializer.Serialize(PetRepo));
        //                    HttpOutput(Encoding.UTF8.GetBytes(serializer.Serialize(PetRepo)), response);
        //                }
        //                else
        //                {
        //                    Console.WriteLine(Hello);
        //                    HttpOutput(Encoding.UTF8.GetBytes(Hello), response);
        //                }
        //            }
        //            else if (request.HttpMethod == "POST")
        //            {
        //                var obj = new Pet()
        //                {
        //                    Name = request.QueryString[request.QueryString.GetKey(0)],
        //                    Type = request.QueryString[request.QueryString.GetKey(1)],
        //                };

        //                PetRepo.Add(obj);

        //                Console.WriteLine(serializer.Serialize(obj));
        //                HttpOutput(Encoding.UTF8.GetBytes(serializer.Serialize(obj)), response);
        //            }
        //        }
        //    }
        //}
        #endregion

        private static void HttpOutput(byte[] buffer, HttpListenerResponse response)
        {
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }
}