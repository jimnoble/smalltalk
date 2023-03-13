using Halforbit.DataStores;
using SmallTalk.Data.Schema;

namespace SmallTalk.Data;

public class SmallTalkDataContext
{
    readonly DataContext _context = new DataContext();

    public IDataStore<IReadOnlyList<User>> Users => _context.Get(c => c
        .LocalStorage()
        .RootPath("c:/smalltalk")
        .YamlSerialization()
        .NoCompression()
        .FileExtension(".yaml")
        .Map<IReadOnlyList<User>>("users"));
}
