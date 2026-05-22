using FluentValidation.Resources;

namespace ReservasXYZ.Infrastructure.Identity;

public class SpanishFluentValidationLanguageManager : LanguageManager
{
    public SpanishFluentValidationLanguageManager()
    {
        AddTranslation("es", "EmailValidator", "'{PropertyName}' no es una dirección de correo válida.");
        AddTranslation("es", "GreaterThanOrEqualValidator", "'{PropertyName}' debe ser mayor o igual a '{ComparisonValue}'.");
        AddTranslation("es", "GreaterThanValidator", "'{PropertyName}' debe ser mayor a '{ComparisonValue}'.");
        AddTranslation("es", "LengthValidator", "'{PropertyName}' debe tener entre {MinLength} y {MaxLength} caracteres. Ingresaste {TotalLength} caracteres.");
        AddTranslation("es", "MinimumLengthValidator", "'{PropertyName}' debe tener al menos {MinLength} caracteres. Ingresaste {TotalLength} caracteres.");
        AddTranslation("es", "MaximumLengthValidator", "'{PropertyName}' no debe superar {MaxLength} caracteres. Ingresaste {TotalLength} caracteres.");
        AddTranslation("es", "LessThanOrEqualValidator", "'{PropertyName}' debe ser menor o igual a '{ComparisonValue}'.");
        AddTranslation("es", "LessThanValidator", "'{PropertyName}' debe ser menor a '{ComparisonValue}'.");
        AddTranslation("es", "NotEmptyValidator", "'{PropertyName}' es obligatorio.");
        AddTranslation("es", "NotEqualValidator", "'{PropertyName}' no debe ser igual a '{ComparisonValue}'.");
        AddTranslation("es", "NotNullValidator", "'{PropertyName}' es obligatorio.");
        AddTranslation("es", "PredicateValidator", "La condición especificada no se cumplió para '{PropertyName}'.");
        AddTranslation("es", "AsyncPredicateValidator", "La condición especificada no se cumplió para '{PropertyName}'.");
        AddTranslation("es", "RegularExpressionValidator", "'{PropertyName}' no tiene el formato correcto.");
        AddTranslation("es", "EqualValidator", "'{PropertyName}' debe ser igual a '{ComparisonValue}'.");
        AddTranslation("es", "ExactLengthValidator", "'{PropertyName}' debe tener exactamente {MaxLength} caracteres. Ingresaste {TotalLength} caracteres.");
        AddTranslation("es", "InclusiveBetweenValidator", "'{PropertyName}' debe estar entre {From} y {To}. Ingresaste {PropertyValue}.");
        AddTranslation("es", "ExclusiveBetweenValidator", "'{PropertyName}' debe estar entre {From} y {To} (exclusivo). Ingresaste {PropertyValue}.");
        AddTranslation("es", "CreditCardValidator", "'{PropertyName}' no es un número de tarjeta de crédito válido.");
        AddTranslation("es", "ScalePrecisionValidator", "'{PropertyName}' no debe tener más de {ExpectedPrecision} dígitos en total, con {ExpectedScale} decimales. Se encontraron {Digits} dígitos y {ActualScale} decimales.");
        AddTranslation("es", "EmptyValidator", "'{PropertyName}' debe estar vacío.");
        AddTranslation("es", "NullValidator", "'{PropertyName}' debe estar vacío.");
        AddTranslation("es", "EnumValidator", "'{PropertyName}' tiene un rango de valores que no incluye '{PropertyValue}'.");

        Culture = new System.Globalization.CultureInfo("es");
    }
}
