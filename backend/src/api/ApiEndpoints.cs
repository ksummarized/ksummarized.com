namespace api;

public static class ApiEndpoints
{
    public static string ApiBase => "api";

    public static class Todo {
        public static string Base => $"{ApiBase}/todo";

        public static class Lists {
            public static string ListsBase => $"{Base}/lists";
            public static string Get => $"{ListsBase}/{{Id}}";
            public static string GetAll => $"{ListsBase}";
            public static string Create => $"{ListsBase}";
            public static string Rename => $"{ListsBase}/{{Id}}";
            public static string Delete => $"{ListsBase}/{{Id}}";
        }

        public static class Tasks {
            public static string TaksBase => $"{Base}/items";
            public static string Get => $"{TaksBase}/{{Id}}";
            public static string GetAll => $"{TaksBase}";
            public static string Create => $"{TaksBase}";
            public static string Update => $"{TaksBase}/{{Id}}";
            public static string Delete => $"{TaksBase}/{{Id}}";
        }

        public static class Tags {
            public static string TagsBase => $"{Base}/tags";
            public static string Get => $"{TagsBase}/{{Id}}";
            public static string GetAll => $"{TagsBase}";
            public static string Create => $"{TagsBase}";
            public static string Update => $"{TagsBase}/{{Id}}";
            public static string Delete => $"{TagsBase}/{{Id}}";
        }
    }
}