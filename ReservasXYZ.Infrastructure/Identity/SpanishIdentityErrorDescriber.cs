using Microsoft.AspNetCore.Identity;

namespace ReservasXYZ.Infrastructure.Identity;

public class SpanishIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DefaultError()
        => Err(nameof(DefaultError), "Ocurrió un error desconocido.");

    public override IdentityError ConcurrencyFailure()
        => Err(nameof(ConcurrencyFailure), "Error de concurrencia. El registro fue modificado por otro proceso.");

    public override IdentityError PasswordMismatch()
        => Err(nameof(PasswordMismatch), "Contraseña incorrecta.");

    public override IdentityError InvalidToken()
        => Err(nameof(InvalidToken), "Token inválido o expirado.");

    public override IdentityError RecoveryCodeRedemptionFailed()
        => Err(nameof(RecoveryCodeRedemptionFailed), "El código de recuperación no es válido.");

    public override IdentityError LoginAlreadyAssociated()
        => Err(nameof(LoginAlreadyAssociated), "Ya existe una cuenta con este proveedor de inicio de sesión.");

    public override IdentityError InvalidUserName(string? userName)
        => Err(nameof(InvalidUserName), $"El nombre de usuario '{userName}' no es válido. Solo puede contener letras o dígitos.");

    public override IdentityError InvalidEmail(string? email)
        => Err(nameof(InvalidEmail), $"El correo '{email}' no es válido.");

    public override IdentityError DuplicateUserName(string userName)
        => Err(nameof(DuplicateUserName), "Este correo ya está registrado.");

    public override IdentityError DuplicateEmail(string email)
        => Err(nameof(DuplicateEmail), "Este correo ya está registrado.");

    public override IdentityError InvalidRoleName(string? role)
        => Err(nameof(InvalidRoleName), $"El nombre de rol '{role}' no es válido.");

    public override IdentityError DuplicateRoleName(string role)
        => Err(nameof(DuplicateRoleName), $"El rol '{role}' ya existe.");

    public override IdentityError UserAlreadyHasPassword()
        => Err(nameof(UserAlreadyHasPassword), "El usuario ya tiene contraseña establecida.");

    public override IdentityError UserLockoutNotEnabled()
        => Err(nameof(UserLockoutNotEnabled), "El bloqueo de cuenta no está habilitado para este usuario.");

    public override IdentityError UserAlreadyInRole(string role)
        => Err(nameof(UserAlreadyInRole), $"El usuario ya pertenece al rol '{role}'.");

    public override IdentityError UserNotInRole(string role)
        => Err(nameof(UserNotInRole), $"El usuario no pertenece al rol '{role}'.");

    public override IdentityError PasswordTooShort(int length)
        => Err(nameof(PasswordTooShort), $"La contraseña debe tener al menos {length} caracteres.");

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        => Err(nameof(PasswordRequiresUniqueChars), $"La contraseña debe contener al menos {uniqueChars} caracteres únicos.");

    public override IdentityError PasswordRequiresNonAlphanumeric()
        => Err(nameof(PasswordRequiresNonAlphanumeric), "La contraseña debe incluir al menos un carácter especial (ej: @, #, !, %).");

    public override IdentityError PasswordRequiresDigit()
        => Err(nameof(PasswordRequiresDigit), "La contraseña debe incluir al menos un número (0-9).");

    public override IdentityError PasswordRequiresLower()
        => Err(nameof(PasswordRequiresLower), "La contraseña debe incluir al menos una letra minúscula (a-z).");

    public override IdentityError PasswordRequiresUpper()
        => Err(nameof(PasswordRequiresUpper), "La contraseña debe incluir al menos una letra mayúscula (A-Z).");

    private static IdentityError Err(string code, string description)
        => new() { Code = code, Description = description };
}
