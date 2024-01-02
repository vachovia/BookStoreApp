﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_UI.WASM.Static
{
    public static class Endpoints
    {
#if DEBUG
        public static string BaseUrl = "https://localhost:44382/";
#else
        public static string BaseUrl = "https://bookstore-api20201128151949.azurewebsites.net/";
#endif
        public static string AuthorsEndpoint = $"{BaseUrl}api/authors/";
        public static string BooksEndpoint = $"{BaseUrl}api/books/";
        public static string RegisterEndpoint = $"{BaseUrl}api/users/register/";
        public static string LoginEndpoint = $"{BaseUrl}api/users/login/";

    }
}
