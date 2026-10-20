namespace EfSargable.Sample.Shared;

// Plain POCOs: no attributes here so each DbContext's mapping is the ONLY source of truth.
public class Customer { public int Id { get; set; } public string Email { get; set; } = ""; public string? Notes { get; set; } }
public class Product  { public int Id { get; set; } public string Sku { get; set; } = ""; public string Code { get; set; } = ""; }
public class Order    { public int Id { get; set; } public string Reference { get; set; } = ""; }
public class Translation { public int Id { get; set; } public string Key { get; set; } = ""; public string Text { get; set; } = ""; }