using System.Globalization;
using System.Text.Json;

namespace ReceiptRecognition.Ollama.ReceiptSchemaProviders;

internal class RetailReceiptSchemaProvider : IReceiptSchemaProvider
{

    private static readonly string _ReceiptDocumentSchemaString = _ReceiptDocumentSchemaStringDraft4;

    private static readonly JsonSerializerOptions _JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public  dynamic ReceiptFormat => JsonSerializer.Deserialize<dynamic>(_ReceiptDocumentSchemaString, _JsonSerializerOptions)!;
    


    #region Schemas
    private const string _ReceiptDocumentSchemaStringDraft7 = """
        {   
          "$schema": "http://json-schema.org/draft-07/schema#",
          "type": "object",
          "properties": {
            "MerchantName": {
              "type": "string"
            },
            "MerchantAddress": {
              "type": "string"
            },
            "PurchaseDate": {
              "type": "string"
            },
            "Total": {
              "type": "object",
              "properties": {
                "Amount": {
                  "type": "number"
                },
                "CurrencyCode": {
                  "type": "string"
                }
              },
              "required": [
                "Amount",
                "CurrencyCode"
              ],
              "additionalProperties": false
            },
            "Taxes": {
              "type": "object",
              "properties": {
                "Amount": {
                  "type": "number"
                },
                "CurrencyCode": {
                  "type": "string"
                }
              },
              "required": [
                "Amount"
              ],
              "additionalProperties": false
            },
            "Tip": {
              "type": "object",
              "properties": {
                "Amount": {
                  "type": "integer"
                },
                "CurrencyCode": {
                  "type": "string"
                }
              },
              "required": [
                "Amount"
              ],
              "additionalProperties": false
            },
            "Items": {
              "type": "array",
              "items": {
                "type": "object",
                "properties": {
                  "Description": {
                    "type": "string"
                  },
                  "Quantity": {
                    "type": "integer"
                  },
                  "UnitPrice": {
                    "type": "object",
                    "properties": {
                      "Amount": {
                        "type": "number"
                      },
                      "CurrencyCode": {
                        "type": "string"
                      }
                    },
                    "required": [
                      "Amount"
                    ],
                    "additionalProperties": false
                  },
                  "LineTotal": {
                    "type": "object",
                    "properties": {
                      "Amount": {
                        "type": "number"
                      },
                      "CurrencyCode": {
                        "type": "string"
                      }
                    },
                    "required": [
                      "Amount"
                    ],
                    "additionalProperties": false
                  }
                },
                "required": [
                  "Description",
                  "Quantity",
                  "UnitPrice",
                  "LineTotal"
                ],
                "additionalProperties": false
              }
            },
            "TaxDetails": {
              "type": "array",
              "items": {
                "type": "object",
                "properties": {
                  "Name": {
                    "type": "string"
                  },
                  "Rate": {
                    "type": "number"
                  },
                  "Amount": {
                    "type": "object",
                    "properties": {
                      "Amount": {
                        "type": "number"
                      },
                      "CurrencyCode": {
                        "type": "string"
                      }
                    },
                    "required": [
                      "Amount"
                    ],
                    "additionalProperties": false
                  }
                },
                "required": [
                  "Name",
                  "Rate",
                  "Amount"
                ],
                "additionalProperties": false
              }
            },
            "Payments": {
              "type": "array",
              "items": {
                "type": "object",
                "properties": {
                  "Method": {
                    "type": "string"
                  },
                  "Amount": {
                    "type": "object",
                    "properties": {
                      "Amount": {
                        "type": "number"
                      },
                      "CurrencyCode": {
                        "type": "string"
                      }
                    },
                    "required": [
                      "Amount"
                    ],
                    "additionalProperties": false
                  }
                },
                "required": [
                  "Method",
                  "Amount"
                ],
                "additionalProperties": false
              }
            }
          },
          "required": [
            "MerchantName",
            "MerchantAddress",
            "PurchaseDate",
            "Total"
          ],
          "additionalProperties": false
        }

        """;

    private const string _ReceiptDocumentSchemaStringDraft4 = """
                {
          "$schema": "http://json-schema.org/draft-04/schema#",
          "description": "",
          "type": "object",
          "properties": {
            "MerchantName": {
              "type": "string",
              "minLength": 1
            },
            "MerchantAddress": {
              "type": "string",
              "minLength": 1
            },
            "PurchaseDate": {
              "type": "string",
              "minLength": 1
            },
            "Total": {
              "type": "object",
              "properties": {
                "Amount": {
                  "type": "number"
                },
                "CurrencyCode": {
                  "type": "string",
                  "minLength": 1
                }
              },
              "required": [
                "Amount",
                "CurrencyCode"
              ]
            },
            "Taxes": {
              "type": "object",
              "properties": {
                "Amount": {
                  "type": "number"
                },
                "CurrencyCode": {
                  "type": "string",
                  "minLength": 1
                }
              },
              "required": [
                "Amount",
                "CurrencyCode"
              ]
            },
            "Tip": {
              "type": "object",
              "properties": {
                "Amount": {
                  "type": "number"
                },
                "CurrencyCode": {
                  "type": "string"
                }
              },
              "required": [
                "Amount",
                "CurrencyCode"
              ]
            },
            "Items": {
              "type": "array",
              "uniqueItems": true,
              "minItems": 1,
              "items": {
                "required": [
                  "Description",
                  "Quantity"
                ],
                "properties": {
                  "Description": {
                    "type": "string",
                    "minLength": 1
                  },
                  "Quantity": {
                    "type": "number"
                  },
                  "UnitPrice": {
                    "type": "object",
                    "properties": {
                      "Amount": {
                        "type": "number"
                      },
                      "CurrencyCode": {
                        "type": "string"
                      }
                    },
                    "required": [
                      "Amount",
                      "CurrencyCode"
                    ]
                  },
                  "LineTotal": {
                    "type": "object",
                    "properties": {
                      "Amount": {
                        "type": "number"
                      },
                      "CurrencyCode": {
                        "type": "string",
                        "minLength": 1
                      }
                    },
                    "required": [
                      "Amount",
                      "CurrencyCode"
                    ]
                  }
                }
              }
            },
            "TaxDetails": {
              "type": "array",
              "uniqueItems": true,
              "minItems": 1,
              "items": {
                "required": [
                  "Name",
                  "Rate"
                ],
                "properties": {
                  "Name": {
                    "type": "string",
                    "minLength": 1
                  },
                  "Rate": {
                    "type": "number"
                  },
                  "Amount": {
                    "type": "object",
                    "properties": {
                      "Amount": {
                        "type": "number"
                      },
                      "CurrencyCode": {
                        "type": "string",
                        "minLength": 1
                      }
                    },
                    "required": [
                      "Amount",
                      "CurrencyCode"
                    ]
                  }
                }
              }
            },
            "Payments": {
              "type": "array",
              "uniqueItems": true,
              "minItems": 1,
              "items": {
                "required": [
                  "Method"
                ],
                "properties": {
                  "Method": {
                    "type": "string",
                    "minLength": 1
                  },
                  "Amount": {
                    "type": "object",
                    "properties": {
                      "Amount": {
                        "type": "number"
                      },
                      "CurrencyCode": {
                        "type": "string",
                        "minLength": 1
                      }
                    },
                    "required": [
                      "Amount",
                      "CurrencyCode"
                    ]
                  }
                }
              }
            }
          },
          "required": [
            "MerchantName",
            "MerchantAddress",
            "PurchaseDate",
            "Total",
            "Items",
            "Taxes",
            "Payments"
          ]
        }

        """;

    public string GetDefaultPrompt(CultureInfo receiptCulture)
    {
        string defaultPrompt = $"""
        You are a strict receipt extraction engine. Extract fields and return ONLY compact JSON with no extra text.

        Rules:
        - Parse: MerchantName, MerchantAddress, PurchaseDate (ISO 8601), Currency (ISO 4217),
          Tax, Total, PaymentMethod, and Items (Description, Quantity, UnitPrice, LineTotal).
        - Amounts are numbers; use null if missing. Do not invent data.
        - If both date and time present, include date part in PurchaseDate; normalize to ISO 8601.
        - Respond with ONLY JSON.
        - Use null for unknown fields. Currency should be an ISO 4217 code when present.
        - Infer Quantity/UnitPrice/LineTotal when clearly stated; otherwise leave null.
        - Quantity is usually near the item, can be in the column or have a suffix or prefix "X".
        - List of items usually follows the header of receipt and preceeds taxes.
        - Culture is indicated as {receiptCulture.EnglishName}.
        - Receipt kind is Retail.
        """;
        return defaultPrompt;

    }
    #endregion


}
