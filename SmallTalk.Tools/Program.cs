using Halforbit.DataStores;
using Newtonsoft.Json.Linq;
using SmallTalk.Data;
using SmallTalk.Data.Schema;
using System.Text.RegularExpressions;
using System.Web;

var epoch = new DateTime(1970, 01, 01, 00, 00, 00, DateTimeKind.Utc);

var slackStore = DataStore
    .Describe()
    .LocalStorage()
    .RootPath(Path.Combine(Directory.GetCurrentDirectory(), "content/slack"))
    .JsonSerialization()
    .NoCompression()
    .FileExtension(".json")
    .Map<DateTime, IReadOnlyList<JObject>>(k => $"{k:yyyy-MM-dd}")
    .Build();

var messageDateRepository = new MessageDateRepository();

var users = new Dictionary<string, string>
{
    ["U8PN87H1U"] = "jimnoble@gmail.com",
    ["U04N4LBE5K4"] = "christopher.j.donlan@gmail.com"
};

var emojis = new Dictionary<string, string>
{
    ["man-shrugging"] = "🤷‍♂️",
    ["confused"] = "😕",
    ["slightly_smiling_face"] = "🙂",
    ["man-facepalming"] = "🤦‍♂️"
};


foreach (var date in await slackStore.ListKeys())
{
    var document = await slackStore.Get(date);

    foreach (var item in document)
    {
        var type = item["type"]?.ToString() ?? string.Empty;

        var time = epoch.Add(TimeSpan.FromSeconds(item["ts"].Value<double>()));

        var text = ProcessText(item["text"]?.ToString() ?? string.Empty);

        var user = item["user"]?.ToString() ?? string.Empty;

        var files = (item["files"] as JArray)?.Select(f => f["name"].ToString()) ?? Array.Empty<string>();

        if (files.Any())
        {
            text += string.Join("\r\n\r\n", files);
        }

        //Console.WriteLine($"{type}: {time}: {users[user]}: {text}");
        
        await messageDateRepository.AppendMessageAsync(
            channel: "alchemy",
            date: time.Date,
            message: new Message(
                MessageId: Guid.NewGuid(),
                Channel: "alchemy",
                Author: users[user],
                WriteTime: time,
                ParentMessageDate: null,
                ParentMessageId: null,
                Content: text,
                Attachments: Array.Empty<Attachment>(),
                Reactions: Array.Empty<Reaction>()));
    }
}

string ProcessText(string text)
{
    text = text.Replace('•', '-');

    text = Regex.Replace(
        text, 
        @"\<(?<Url>.*)\|(?<Label>.*)\>", 
        m => $"[{m.Groups["Label"].Value}]({m.Groups["Url"].Value})");

    text = Regex.Replace(
        text, 
        ":(?<Name>[a-zA-Z0-9_-]+):",
        m => emojis[m.Groups["Name"].Value]);

    var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

    for (var i = 0; i < lines.Count; i++)
    {
        var line = lines[i];

        if (line.StartsWith("```"))
        {
            if (line.EndsWith("```"))
            {
                lines[i] = $"```\n{line.Substring(3, line.Length - 6)}\n```";
            }
            else
            {
                lines[i] = $"```\n{line.Substring(3)}";
            }
        }
        else if (line.EndsWith("```"))
        {
            lines[i] = $"{line.Substring(0, line.Length - 3)}\n```";
        }
    }

    return HttpUtility.HtmlDecode(string.Join("\n", lines));
}