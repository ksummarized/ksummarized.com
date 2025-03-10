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
            public static string Rename => $"{Base}/{{id}}";
            public static string Delete => $"{Base}/{{id}}";
        }

        public static class Tasks {
            public static string Base => $"{Todo.Base}/tasks";
            public static string Get => $"{Base}/{{id}}";
            public static string GetAll => $"{Base}";
            public static string Create => $"{Base}";
            public static string Update => $"{Base}/{{id}}";
            public static string Delete => $"{Base}/{{id}}";
        }
    }
}