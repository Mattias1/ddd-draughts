Todo list
----------
Things to do before making this a nuget package (or version 1.0.0):

- Wrap starting a transaction with my own exception type
  - Other places that I haven't caught?
- Advanced joins (where's, orJoin?)
- Union
- QueryBuilder inside a `Where(q => q.x)` should _only_ allow other where functions, and no orderby for example
  (can having use the same?)
- Having with count, max, etc. functions
- Option: Capitalisation of keywords
- Documentation comments `///`
- SqlServer, PostgreSql
- Integration tests for the readme example queries
