using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public class ListRenameRequest
{
    [FromRoute]
    public required int Id { get; set; }

    [FromBody]
    public required Payload Body { get; set; }

    public class Payload
    {
        public required string Name { get; set; }
    }
}
