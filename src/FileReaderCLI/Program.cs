using FileReaderLibrary;
using System.Text.Json;
using System.Xml.Linq;

var reader = new FileReader();

Console.WriteLine("File Reader CLI");
Console.WriteLine("----------------");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Choose an option:");
    Console.WriteLine("1) Read Text");
    Console.WriteLine("2) Read XML");
    Console.WriteLine("3) Read JSON");
    Console.WriteLine("X) Exit");
    Console.Write("Selection: ");
    var choice = (Console.ReadLine() ?? string.Empty).Trim();

    if (string.Equals(choice, "x", StringComparison.OrdinalIgnoreCase))
        break;

    string fileType = choice switch
    {
        "1" => "text",
        "2" => "xml",
        "3" => "json",
        _ => string.Empty
    };

    if (string.IsNullOrEmpty(fileType))
    {
        Console.WriteLine("Invalid selection.");
        continue;
    }

    Console.Write("Enter file path: ");
    var path = (Console.ReadLine() ?? string.Empty).Trim();

    if (string.IsNullOrWhiteSpace(path))
    {
        Console.WriteLine("Path is required.");
        continue;
    }

    Console.Write("Use encryption (y/N)? ");
    var useEnc = IsYes(Console.ReadLine());

    Console.Write("Use role-based security (y/N)? ");
    var useAuth = IsYes(Console.ReadLine());

    string? role = null;
    if (useAuth)
    {
        Console.Write("Enter role (e.g., admin or user): ");
        role = (Console.ReadLine() ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(role))
        {
            Console.WriteLine("Role is required when role-based security is enabled.");
            continue;
        }
    }

    try
    {
        if (fileType == "text")
        {
            await HandleTextAsync(reader, path, useEnc, useAuth, role);
        }
        else if (fileType == "xml")
        {
            await HandleXmlAsync(reader, path, useEnc, useAuth, role);
        }
        else if (fileType == "json")
        {
            await HandleJsonAsync(reader, path, useEnc, useAuth, role);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.GetType().Name}: {ex.Message}");
    }

    Console.WriteLine();
    Console.Write("Read another file (Y/n)? ");
    if (!IsYes(Console.ReadLine(), defaultYes: true))
        break;
}

static bool IsYes(string? input, bool defaultYes = false)
{
    var trimmed = (input ?? string.Empty).Trim();
    if (string.IsNullOrEmpty(trimmed)) return defaultYes;
    return string.Equals(trimmed, "y", StringComparison.OrdinalIgnoreCase) ||
           string.Equals(trimmed, "yes", StringComparison.OrdinalIgnoreCase);
}

static async Task HandleTextAsync(FileReader reader, string path, bool useEnc, bool useAuth, string? role)
{
    string content;
    if (useEnc)
    {
        if (useAuth)
        {
            EnsureAuthorizedForText(path, role!);
        }
        var decryptor = new ReverseTextDecryptor();
        content = await reader.ReadEncryptedTextAsync(path, decryptor);
    }
    else
    {
        if (useAuth)
        {
            var authorizer = new SimpleRoleTextAccessAuthorizer(new[] { path });
            content = await reader.ReadAllTextAuthorizedAsync(path, role!, authorizer);
        }
        else
        {
            content = await reader.ReadAllTextAsync(path);
        }
    }

    Console.WriteLine("--- TEXT CONTENT START ---");
    Console.WriteLine(content);
    Console.WriteLine("--- TEXT CONTENT END ---");
}

static async Task HandleXmlAsync(FileReader reader, string path, bool useEnc, bool useAuth, string? role)
{
    XDocument doc;
    if (useEnc)
    {
        if (useAuth)
        {
            EnsureAuthorizedForXml(path, role!);
        }
        var decryptor = new ReverseTextDecryptor();
        doc = await reader.ReadEncryptedXmlAsync(path, decryptor);
    }
    else
    {
        if (useAuth)
        {
            var authorizer = new SimpleRoleXmlAccessAuthorizer(new[] { path });
            doc = await reader.ReadXmlAuthorizedAsync(path, role!, authorizer);
        }
        else
        {
            doc = await reader.ReadXmlAsync(path);
        }
    }

    Console.WriteLine("--- XML CONTENT START ---");
    Console.WriteLine(doc.ToString(SaveOptions.None));
    Console.WriteLine("--- XML CONTENT END ---");
}

static async Task HandleJsonAsync(FileReader reader, string path, bool useEnc, bool useAuth, string? role)
{
    JsonDocument doc;
    if (useEnc)
    {
        if (useAuth)
        {
            EnsureAuthorizedForJson(path, role!);
        }
        var decryptor = new ReverseTextDecryptor();
        doc = await reader.ReadEncryptedJsonAsync(path, decryptor);
    }
    else
    {
        if (useAuth)
        {
            var authorizer = new SimpleRoleJsonAccessAuthorizer(new[] { path });
            doc = await reader.ReadJsonAuthorizedAsync(path, role!, authorizer);
        }
        else
        {
            doc = await reader.ReadJsonAsync(path);
        }
    }

    var options = new JsonSerializerOptions { WriteIndented = true };
    var pretty = JsonSerializer.Serialize(doc.RootElement, options);

    Console.WriteLine("--- JSON CONTENT START ---");
    Console.WriteLine(pretty);
    Console.WriteLine("--- JSON CONTENT END ---");
}

static void EnsureAuthorizedForText(string path, string role)
{
    var authorizer = new SimpleRoleTextAccessAuthorizer(new[] { path });
    if (!authorizer.CanRead(path, role))
        throw new UnauthorizedAccessException($"Role '{role}' is not authorized to read '{path}'.")
    ;
}

static void EnsureAuthorizedForXml(string path, string role)
{
    var authorizer = new SimpleRoleXmlAccessAuthorizer(new[] { path });
    if (!authorizer.CanRead(path, role))
        throw new UnauthorizedAccessException($"Role '{role}' is not authorized to read '{path}'.")
    ;
}

static void EnsureAuthorizedForJson(string path, string role)
{
    var authorizer = new SimpleRoleJsonAccessAuthorizer(new[] { path });
    if (!authorizer.CanRead(path, role))
        throw new UnauthorizedAccessException($"Role '{role}' is not authorized to read '{path}'.")
    ;
}
