Todo list
----------
Things to do before making this a nuget package (or version 1.0.0):

- Wrap starting a transaction with my own exception type
  - Other places that I haven't caught?
- Advanced joins (where's, orJoin?)
- Subqueries (in, exists, =, >, etc.)
- Union
- Insert into select, update from select (Maybe this already works? Make tests then?)
- QueryBuilder inside a `Where(q => q.x)` should _only_ allow other where functions, and no orderby for example
  (can having use the same?)
- Having with count, max, etc. functions
- Option: Capitalisation of keywords
- Option: Throw for delete and update queries without where (and add a 'WithoutWhere()' method?)
  - Needs documentation and a very friendly exception ;)
- Documentation comments `///`
- SqlServer, PostgreSql
- Integration tests for the readme example queries
