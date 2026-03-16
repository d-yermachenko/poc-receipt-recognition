using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Google.Protobuf.WellKnownTypes;
using k8s;
using ReceiptRecognition.Core;

namespace ReceiptRecognition.Ollama.ReceiptSchemaProviders;

internal class RestaurantReceiptSchemaProvider : IReceiptSchemaProvider
{

    private static readonly string _ReceiptDocumentSchemaString = _ReceiptDocumentSchemaStringDraft4;

    private static readonly JsonSerializerOptions _JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public dynamic ReceiptFormat =>  JsonSerializer.Deserialize<dynamic>(_ReceiptDocumentSchemaString, _JsonSerializerOptions)!;

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
        You are a strict restaurant receipt extraction engine. Output ONLY compact JSON that matches the expected schema; no extra text.

        Output keys:
        -MerchantName, MerchantAddress, PurchaseDate, Items, Taxes, Tip(optional), Total, Payments, TaxDetails(optional)

        Rules:
        -PurchaseDate: ISO 8601 date(YYYY - MM - DD).If a date-time is present, keep both.
        -Money fields(Taxes, Tip, Total, UnitPrice, LineTotal): objects with Amount(number) and CurrencyCode(ISO 4217, uppercase) or Symbol(like $, €, £). Use null when unknown.
        -Items: capture each menu line with Description, Quantity, UnitPrice, LineTotal.
          -If Quantity is not printed, default to 1.
          - Detect quantities like: "2x", "x2", "2 X", "Qty 2", "QTY:2".
          - If two of[Quantity, UnitPrice, LineTotal] are present, infer the third; otherwise leave it null.
          - Modifiers / add - ons with zero price: append to the parent item Description(do not add a separate priced line).
          - Discounts / comps / voids: include as items with negative LineTotal; Quantity 1 unless printed.
          - Service / cover charges: if priced, include as items; if a tax with a visible rate, also add an entry in TaxDetails.
        - Taxes: set Taxes to the sum of printed taxes. If multiple components exist, also provide TaxDetails with Name, Rate, Amount when visible; omit TaxDetails if not visible.
        - Tip: use the actual paid tip if present; ignore suggested tip tables.
        - Payments: array of tenders, each with Method(e.g., "Cash", "Visa", "Ticket restauraunt" and its abbreveations like T/R) and Amount. For split payments, include multiple entries.
        - Respond with ONLY JSON using exactly the keys above. Do not add extra fields.
        - Culture is { receiptCulture.EnglishName }; interpret localized tax / total words(e.g., TVA, GST, MwSt, IVA, Service, Propina).

        """;
        return defaultPrompt;
        return defaultPrompt;

    }

    public ReceiptDto? SerializeToReceipt(string content)
    {
        return JsonSerializer.Deserialize<ReceiptDto?>(content, _JsonSerializerOptions);
    }
    #endregion


}
