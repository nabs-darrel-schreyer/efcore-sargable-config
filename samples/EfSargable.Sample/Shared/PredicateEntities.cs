namespace EfSargable.Sample.Shared;

// Entities for the non-sargable predicate rules (article: "Non-sargable predicates").
public class Person  { public int Id { get; set; } public string FirstName { get; set; } = ""; public string LastName { get; set; } = ""; public string ReversedLastName { get; private set; } = ""; }
public class Sale    { public int Id { get; set; } public DateTime OrderDate { get; set; } public DateTime CreatedAt { get; set; } public decimal Price { get; set; } public string? Status { get; set; } }
public class Account { public int Id { get; set; } public string AccountNumber { get; set; } = ""; }