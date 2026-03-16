using System.Globalization;
using System.Text.Json;
using ReceiptRecognition.Core;

namespace ReceiptRecognition.Ollama.ReceiptSchemaProviders;

public sealed record FuelReceiptDto(
    string? StationName,
    string? StationAddress,
    string? PurchaseDate,  // ISO 8601 date inferred
    Currency? Total,
    Currency? Taxes,
    Currency? Tip,
    string? FuelType,
    decimal? Volume,
    string? VolumeUnit,
    Currency? Price,
    IReadOnlyList<ReceiptTaxDetailDto> TaxDetails,
    IReadOnlyList<ReceiptPaymentDto> Payments
);


internal class GasStationReceiptSchemaProvider : IReceiptSchemaProvider
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
          "type": "object",
          "properties": {
            "StationName": {
              "type": "string",
              "description" : "Name of gas station"
            },
            "StationAddress": {
              "type": "string",
              "description" : "Address of gas station"
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
              ]
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
                "Amount",
                "CurrencyCode"
              ]
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
                "Amount",
                "CurrencyCode"
              ]
            },
            "FuelType": {
              "type": "string"
            },
            "Volume": {
              "type": "number"
            },
            "VolumeUnit": {
              "type": "string"
            },
            "Price": {
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
            "TaxDetails": {
              "type": "array",
              "items": [
                {
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
                        "Amount",
                        "CurrencyCode"
                      ]
                    }
                  },
                  "required": [
                    "Name",
                    "Rate",
                    "Amount"
                  ]
                }
              ]
            },
            "Payments": {
              "type": "array",
              "items": [
                {
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
                        "Amount",
                        "CurrencyCode"
                      ]
                    }
                  },
                  "required": [
                    "Method",
                    "Amount"
                  ]
                }
              ]
            }
          },
          "required": [
            "StationName",
            "StationAddress",
            "PurchaseDate",
            "Total",
            "Taxes",
            "FuelType",
            "Volume",
            "VolumeUnit",
            "Price",
            "TaxDetails",
            "Payments"
          ]
        }
        """;

    public string GetDefaultPrompt(CultureInfo receiptCulture)
    {
        string defaultPrompt = $"""
        You are a strict receipt extraction engine. Extract fields and return ONLY compact JSON with no extra text.

        Rules:
        - Receipt kind is Gas station ticket.
        - Gas station is the Merchant.
        - Parse: MerchantName, MerchantAddress, PurchaseDate (ISO 8601), Currency (ISO 4217),
          Tax, Total, PaymentMethod, and Items (Description, Quantity, UnitPrice, LineTotal).
        - Amounts are numbers; use null if missing. Do not invent data.
        - If both date and time present, include date part in PurchaseDate; normalize to ISO 8601.
        - Respond with ONLY JSON.
        - Use null for unknown fields. Currency should be an ISO 4217 code when present.
        - Infer Quantity/UnitPrice/LineTotal when clearly stated; otherwise leave null.
        - Quantity is usually near the item, can be in the column or have a suffix or prefix of local volume unit.
        - List of items usually follows the header of receipt and preceeds taxes.
        - Culture is indicated as {receiptCulture.EnglishName}.
        
        """;
        return defaultPrompt;

    }

    public ReceiptDto? SerializeToReceipt(string? content)
    {
        if (String.IsNullOrEmpty(content))
            return null;
        FuelReceiptDto? receiptData = JsonSerializer.Deserialize<FuelReceiptDto>(content);
        ReceiptItemDto receiptItem = new (receiptData?.FuelType, receiptData?.Volume, receiptData?.Price, receiptData?.Total);
        ReceiptDto gasReceipt = new(receiptData?.StationName,
            receiptData?.StationAddress,
            receiptData?.PurchaseDate,
            receiptData?.Total,
            receiptData?.Taxes,
            receiptData?.Tip,
            [receiptItem],
            receiptData?.TaxDetails ?? [],
            receiptData?.Payments ?? []);
        return gasReceipt;
    }
    #endregion


}
