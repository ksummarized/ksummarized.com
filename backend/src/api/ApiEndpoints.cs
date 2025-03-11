namespace api;

public static class ApiEndpoints
{
    public static string ApiBase => "api";

    public static class Todo {
        public static string Base => $"{ApiBase}/todo";

        public static class Lists {
            public static string Base => $"{Todo.Base}/lists";
            public static string Get => $"{Base}/{{Id}}";
            public static string GetAll => $"{Base}";
            public static string Create => $"{Base}";
            public static string Rename => $"{Base}/{{Id}}";
            public static string Delete => $"{Base}/{{Id}}";
        }

        public static class Tasks {
            public static string Base => $"{Todo.Base}/items";
            public static string Get => $"{Base}/{{Id}}";
            public static string GetAll => $"{Base}";
            public static string Create => $"{Base}";
            public static string Update => $"{Base}/{{Id}}";
            public static string Delete => $"{Base}/{{Id}}";
        }
    }
}