## DawnIQureryable

This class provides some extension functions to generate SQL to query database record.

- **WhereSearch**

  **Declare**:

  ```C#
  public static IQueryable<TEntity> WhereMatch<TEntity>(
      this IQueryable<TEntity> @this,
      string searchString, 
      Expression<Func<TEntity, object>> searchMembers)
  ```

  Queries records which is contains the specified string in any fields.

  For example, if you want to query ***Bill*** in field ***First_Name*** or ***Last_Name***:

  ```C#
  _context.Employees.WhereSearch("Bill", x => new { x.First_Name, x.Last_Name });
  ```

  This invoke will generate a SQL query string like this:

  ```SQL
  SELECT [x].[Id], [x].[First_Name], [x].[Last_Name]
  FROM [Emplyees] AS [x]
  WHERE (CHARINDEX(N'Bill', [x].[First_Name]) > 0)
  	OR (CHARINDEX(N'Bill', [x].[Last_Name]) > 0);
  ```

- **WhereMatch**
  Different from **WhereMatch**, this statement will perform an exact match:

  ```SQL
  SELECT [x].[Id], [x].[First_Name], [x].[Last_Name]
  FROM [Emplyees] AS [x]
  WHERE [x].[First_Name] = N'Bill' OR [x].[Last_Name] = N'Bill'
  ```

- **WhereMax**
  WhereMax

- **WhereMin**
  WhereMin

- **WhereInRange**
  (Not supportted yet)
  Queries records which is start at a time and end at another time.
