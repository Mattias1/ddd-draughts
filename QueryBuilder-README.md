SQL Query builder
------------------
A lightweight querybuilder for my database interactions.

Since Linq to SQL is no longer supported and something like NHibernate or Entity framework is way
overkill all that's left are some simple query builders or wrappers around Dapper.
So naturally I decided to add another query builder to the pile ;)

Examples
---------
A simple search query:
```
public IInitialQueryBuilder Query() {
    var sqlFlavor = new MySqlFlavor("localhost", "sql_user", "sql_password", "sql_database");
    return QueryBuilder.Init(sqlFlavor);
}

public IReadOnlyList<UserTable> Search(string name) {
    return Query()
        .SelectAllFrom("user")
        .Where("username").Like($"%{name}%")
        .OrderByDesc("created_at")
        .List<UserTable>();
    // Executes "select user.* from user where username like @0 order by created_at desc"
}
```

Or maybe you'd like to:
```
public bool SaveUser(UserModel model) {
    return Query()
        .InsertInto("user")
        .InsertFrom(model)
        .Execute();
    // Executes "insert into user (id, username, email, created_at) values (42, @0, @1, @2)"
}

public class UserModel {
    public long Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public LocalDateTime CreatedAt { get; set; }
}
```
Note that the id is not parameterized, because it's a `long` type, and therefore safe. This will
give you a performance boost for large where in lists.
You can turn this off if you want (provide an option in the Init).

A more complicated example:
```
public async Task<IReadOnlyList<GroupingModel>> NewUsersWithManyRoles() {
    var query = Query()
        .Select().Column("u.id").Column("u.username").CountAs("r.id", "roles")
        .FromAs("user", "u")
        .JoinAs("role_user", "ru", "u.id", "ru.user_id")
        .JoinAs("role", "r", "ru.role_id", "r.id")
        .Where(q => q
            .Where("u.created_at").Gt(new LocalDate(2020, 02, 29))
            .Or("username").Is("moderator") // As benchmark
        )
        .AndNot(q => q
            .Where("u.id").Is(1)
            .Or("u.username").Is("admin")
        )
        .GroupBy("u.id", "u.username")
        .Having("roles").GtEq(3)
        .OrderByAsc("roles");

    string rawUnsafeSql = query.ToUnsafeSql();
    // The executed sql, with parameters inserted for debugging purposes, shows us the following:
    // select u.id, u.username, count(r.id) as roles
    // from user as u
    // join role_user as ru on u.id = ru.user_id
    // join role as r on ru.role_id = r.id
    // where (
    //     u.created_at >= '2020-03-01'
    //     or username = 'moderator'
    // )
    // and not (
    //     u.id = 1
    //     or u.username = 'admin'
    // )
    // group by u.id, u.username
    // having roles >= 3
    // order by roles asc

    return await query.ListAsync<GroupingModel>();
}
```
Note that the date check `if date > feb 29` is transformed to `if date >= march 01`, to make sure
that noon feb 29 for example is not included in the check. This is only done for a `LocalDate`, not
for any other date types, like `LocalDateTime` or `System.DateTime` (if you can't user NodaTime) for
example.
Again, if you don't like this, you can turn it off as option.
