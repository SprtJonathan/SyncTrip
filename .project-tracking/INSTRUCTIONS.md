# SyncTrip - Instructions pour le Développement

**Version** : 1.2
**Date** : 13 Février 2026

---

## Règles Absolues (Non Négociables)

### 1. Commits Fréquents et Atomiques

✅ **TOUJOURS faire un commit après** :
- Création d'une entité complète et compilable
- Création d'un DTO complet
- Implémentation d'un handler avec son validator
- Ajout d'un controller avec ses endpoints
- Création d'une View Avalonia avec son ViewModel
- Correction d'une erreur de compilation
- Ajout d'un test qui passe

❌ **NE JAMAIS** :
- Faire un commit avec du code qui ne compile pas
- Attendre d'avoir fini toute une feature avant de commit
- Faire des commits avec des messages vagues ("wip", "update", "fix")

**Format des messages de commit** :
```
<type>(<scope>): <description>

Types valides :
- feat: Nouvelle fonctionnalité
- fix: Correction de bug
- refactor: Refactoring sans changement fonctionnel
- test: Ajout ou modification de tests
- docs: Documentation
- chore: Tâches diverses (config, etc.)

Exemples :
feat(core): ajoute l'entité User avec validation âge > 14 ans
feat(shared): ajoute les DTOs d'authentification
feat(application): ajoute le handler SendMagicLinkCommand
feat(api): ajoute AuthController avec endpoint magic-link
feat(mobile): ajoute MagicLinkPage et ViewModel
test(core): ajoute tests validation âge User
fix(application): corrige validation email dans CompleteRegistration
```

---

### 2. Compilation Sans Erreur OBLIGATOIRE

**Avant CHAQUE commit** :
```bash
# Compiler toute la solution
dotnet build SyncTrip.sln

# Vérifier qu'il n'y a AUCUNE erreur
# Warnings acceptables, erreurs NON
```

**Si erreur de compilation** :
1. Corriger immédiatement
2. Re-compiler
3. Puis faire le commit

**Si feature non terminée mais besoin de commit** :
- Commenter temporairement le code non compilable
- OU créer une branche `wip/feature-name` (à merger seulement quand ça compile)

---

### 3. Validation d'une Feature Terminée

Une feature n'est **TERMINÉE** que si :

✅ **Checklist Obligatoire** :
- [ ] Tous les fichiers de la feature sont créés
- [ ] Le code compile sans erreur (`dotnet build` succès)
- [ ] Les tests unitaires passent (`dotnet test` succès)
- [ ] La feature a été testée manuellement (si applicable)
- [ ] La documentation est à jour (PROGRESS.md, ARCHITECTURE.md si changements)
- [ ] Tous les commits sont faits avec des messages clairs
- [ ] Le code suit les conventions du projet

**Procédure de validation** :
```bash
# 1. Compiler
dotnet build SyncTrip.sln

# 2. Tester
dotnet test

# 3. Vérifier git status
git status

# 4. Si tout est OK, mettre à jour PROGRESS.md
# 5. Commit final de la feature
git add .
git commit -m "feat: feature X terminée et validée"
```

---

## Workflow de Développement

### Approche Verticale (Vertical Slice)

Pour chaque feature, développer dans cet ordre :

**Étape 1 : Core (Domain)**
1. Créer les entités
2. Créer les interfaces
3. Créer les value objects si besoin
4. **Commit** : `feat(core): ajoute entités pour feature X`

**Étape 2 : Shared (DTOs)**
1. Créer les DTOs Request
2. Créer les DTOs Response
3. Créer les enums partagés
4. **Commit** : `feat(shared): ajoute DTOs pour feature X`

**Étape 3 : Application (Use Cases)**
1. Créer les Commands/Queries
2. Créer les Handlers
3. Créer les Validators (FluentValidation)
4. Mapping Entity → DTO manuellement dans les handlers
5. Créer les interfaces de services cross-layer si nécessaire (ex: ITripNotificationService)
6. **Commits multiples** :
   - `feat(application): ajoute command X et handler`
   - `feat(application): ajoute validator pour X`

**Étape 4 : Infrastructure**
1. Créer les repositories
2. Créer les configurations EF Core (IEntityTypeConfiguration)
3. Créer les services (BackgroundService si besoin)
4. Mettre à jour ApplicationDbContext (DbSet)
5. Mettre à jour DependencyInjection.cs
6. Créer la migration EF Core :
   ```bash
   dotnet ef migrations add NomMigration --project "src/SyncTrip.Infrastructure" --startup-project "src/SyncTrip.API"
   ```
7. **Commits multiples** :
   - `feat(infrastructure): ajoute repository X`
   - `feat(infrastructure): ajoute service Y`
   - `feat(infrastructure): ajoute migration pour feature X`

**Étape 5 : API**
1. Créer le controller
2. Ajouter les endpoints
3. Configurer DI dans Program.cs si besoin
4. **Commit** : `feat(api): ajoute controller X avec endpoints`

**Étape 6 : Mobile**
1. Créer le service (si besoin)
2. Créer la View (XAML)
3. Créer le ViewModel
4. Configurer la navigation
5. **Commits multiples** :
   - `feat(mobile): ajoute service X`
   - `feat(mobile): ajoute page Y et ViewModel`

**Étape 7 : Tests**
1. Tests entités (Core.Tests)
2. Tests handlers (Application.Tests)
3. Tests API (Integration tests si applicable)
4. **Commit** : `test: ajoute tests pour feature X`

**Étape 8 : Validation Finale**
1. Compiler toute la solution
2. Lancer tous les tests
3. Tester manuellement
4. Mettre à jour PROGRESS.md
5. **Commit** : `feat: feature X terminée et validée`

---

## Structure Git

### Branches

**Branches principales** :
- `main` : Production-ready (protégée)
- `develop` : Intégration

**Branches de feature** :
```
feat/auth-magic-link
feat/profile-garage
feat/convoy-system
feat/trip-gps
feat/voting-system
feat/chat
```

**Workflow** :
```bash
# Créer une branche de feature
git checkout develop
git pull
git checkout -b feat/nom-feature

# Travailler avec commits fréquents
git add .
git commit -m "feat(scope): description"

# Pousser régulièrement
git push -u origin feat/nom-feature

# Une fois terminé, merger dans develop
git checkout develop
git merge feat/nom-feature
git push

# Supprimer la branche de feature
git branch -d feat/nom-feature
git push origin --delete feat/nom-feature
```

---

## Conventions de Code

### C# (.NET)

**Entités** :
```csharp
// ✅ BON
public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }

    // Constructeur privé pour EF Core
    private User() { }

    // Factory method
    public static User Create(string email, string username, DateOnly birthDate)
    {
        // Validation
        if (CalculateAge(birthDate) <= 14)
            throw new DomainException("L'utilisateur doit avoir plus de 14 ans");

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            // ...
        };
    }
}

// ❌ MAUVAIS
public class User
{
    public Guid Id { get; set; } // Public setter
    public string Email; // Champ public
}
```

**DTOs** :
```csharp
// ✅ BON - Record immutable
public record CreateConvoyRequest(bool IsPrivate);

// ✅ BON - Classe avec init
public class ConvoyDto
{
    public Guid Id { get; init; }
    public string JoinCode { get; init; }
}
```

**Handlers MediatR** :
```csharp
// ✅ BON
public class SendMagicLinkCommandHandler : IRequestHandler<SendMagicLinkCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public SendMagicLinkCommandHandler(
        IUserRepository userRepository,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task Handle(SendMagicLinkCommand request, CancellationToken ct)
    {
        // Logique
    }
}
```

**Validators FluentValidation** :
```csharp
// ✅ BON
public class CompleteRegistrationValidator : AbstractValidator<CompleteRegistrationCommand>
{
    public CompleteRegistrationValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Le pseudo est obligatoire")
            .MaximumLength(50);

        RuleFor(x => x.BirthDate)
            .Must(BeOlderThan14)
            .WithMessage("Vous devez avoir plus de 14 ans");
    }

    private bool BeOlderThan14(DateOnly birthDate)
    {
        var age = DateTime.UtcNow.Year - birthDate.Year;
        return age > 14;
    }
}
```

---

### AXAML (AvaloniaUI)

**View AXAML** :
```xml
<!-- ✅ BON -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="clr-namespace:SyncTrip.App.Features.Auth.ViewModels"
             x:Class="SyncTrip.App.Features.Auth.Views.MagicLinkView"
             x:DataType="vm:MagicLinkViewModel">
    <StackPanel Margin="20">
        <TextBox Watermark="Votre email"
                 Text="{Binding Email}" />
        <Button Content="Envoyer le lien"
                Command="{Binding SendMagicLinkCommand}" />
    </StackPanel>
</UserControl>
```

**ViewModel** :
```csharp
// ✅ BON
public partial class MagicLinkViewModel : ObservableObject
{
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    private string email = string.Empty;

    public MagicLinkViewModel(IAuthenticationService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task SendMagicLink()
    {
        // Logique
    }
}
```

---

## Gestion des Erreurs

### API

```csharp
// ✅ BON - Utiliser Result<T> ou exceptions métier
public async Task<IActionResult> CreateConvoy([FromBody] CreateConvoyRequest request)
{
    try
    {
        var command = new CreateConvoyCommand(UserId, request.IsPrivate);
        var convoyId = await _mediator.Send(command);
        return Ok(new { ConvoyId = convoyId });
    }
    catch (ValidationException ex)
    {
        return BadRequest(new { Errors = ex.Errors });
    }
    catch (DomainException ex)
    {
        return BadRequest(new { Message = ex.Message });
    }
}
```

### Mobile

```csharp
// ✅ BON - Try/catch avec message utilisateur
[RelayCommand]
private async Task CreateConvoy()
{
    try
    {
        IsBusy = true;
        var result = await _convoyService.CreateConvoyAsync(IsPrivate);
        await Shell.Current.GoToAsync($"//convoy/{result.ConvoyId}");
    }
    catch (Exception ex)
    {
        await Shell.Current.DisplayAlert("Erreur",
            "Impossible de créer le convoi. Veuillez réessayer.",
            "OK");
    }
    finally
    {
        IsBusy = false;
    }
}
```

---

## Checklist Avant de Déclarer une Feature Terminée

### Core
- [ ] Entités créées avec constructeurs privés + factory methods
- [ ] Validation métier dans les entités
- [ ] Interfaces définies
- [ ] Pas de dépendances externes
- [ ] Compile sans erreur

### Shared
- [ ] DTOs Request créés
- [ ] DTOs Response créés
- [ ] Enums définis
- [ ] Compile sans erreur

### Application
- [ ] Commands/Queries créés
- [ ] Handlers implémentés (mapping Entity → DTO manuel)
- [ ] Validators FluentValidation créés
- [ ] Interfaces de services cross-layer si nécessaire
- [ ] Tests unitaires passent
- [ ] Compile sans erreur

### Infrastructure
- [ ] Repositories implémentés
- [ ] Services implémentés
- [ ] Configurations EF Core créées
- [ ] Migration créée et appliquée
- [ ] DependencyInjection.cs à jour
- [ ] Compile sans erreur

### API
- [ ] Controller créé
- [ ] Endpoints documentés (Scalar / OpenAPI)
- [ ] Authentication configurée ([Authorize])
- [ ] Services d'implémentation cross-layer si nécessaire (ex: TripNotificationService)
- [ ] DI configuré dans Program.cs
- [ ] Tests d'intégration (si applicable)
- [ ] Compile sans erreur

### Mobile
- [ ] Service créé (si besoin)
- [ ] View (XAML) créée
- [ ] ViewModel créé avec CommunityToolkit.Mvvm
- [ ] Navigation configurée
- [ ] DI configuré dans App.axaml.cs (services, VMs, routes)
- [ ] Testé sur au moins une plateforme
- [ ] Compile sans erreur

### Documentation
- [ ] PROGRESS.md mis à jour
- [ ] ARCHITECTURE.md mis à jour (si changements structurels)
- [ ] Commits avec messages clairs

---

## Outils de Vérification

### Compilation
```bash
# Compiler toute la solution
dotnet build SyncTrip.sln

# Compiler un projet spécifique
dotnet build src/SyncTrip.Core/SyncTrip.Core.csproj
```

### Tests
```bash
# Tous les tests
dotnet test

# Tests d'un projet spécifique
dotnet test tests/SyncTrip.Core.Tests/

# Avec couverture de code
dotnet test /p:CollectCoverage=true
```

### Migrations EF Core
```bash
# Ajouter une migration
dotnet ef migrations add MigrationName \
  --project src/SyncTrip.Infrastructure \
  --startup-project src/SyncTrip.API

# Appliquer les migrations
dotnet ef database update \
  --project src/SyncTrip.Infrastructure \
  --startup-project src/SyncTrip.API

# Générer un script SQL
dotnet ef migrations script \
  --project src/SyncTrip.Infrastructure \
  --startup-project src/SyncTrip.API \
  --output migration.sql
```

---

## Résolution de Problèmes Courants

### "Le projet ne compile pas"
1. Vérifier les références de projets
2. Vérifier les using statements
3. Vérifier les packages NuGet installés
4. Nettoyer et rebuilder : `dotnet clean && dotnet build`

### "Les tests ne passent pas"
1. Vérifier que la base de données de test est propre
2. Vérifier les mocks (Moq)
3. Vérifier les assertions
4. Lancer les tests un par un pour isoler le problème

### "Erreur de migration EF Core"
1. Vérifier que PostgreSQL est démarré
2. Vérifier la connection string
3. Supprimer la dernière migration : `dotnet ef migrations remove`
4. Recréer la migration

### "Problème SignalR Mobile"
1. Vérifier l'URL du hub
2. Vérifier le JWT dans la connexion
3. Vérifier les logs côté API
4. Tester avec Postman d'abord

---

## Contacts & Ressources

**Documentation de référence** :
- DOCUMENTATION.md : Spécifications fonctionnelles
- ARCHITECTURE.md : Architecture technique
- PROGRESS.md : Suivi de progression

**Ressources externes** :
- [Clean Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- [AvaloniaUI Docs](https://docs.avaloniaui.net/)
- [MediatR](https://github.com/jbogard/MediatR)
- [FluentValidation](https://docs.fluentvalidation.net/)

---

**Dernière mise à jour** : 13 Février 2026

---

## Rappel Final

🔴 **IMPÉRATIF** :
1. Commits fréquents après chaque fichier/groupe compilable
2. TOUJOURS compiler avant de commit
3. Une feature n'est terminée que si elle compile + tests passent
4. Messages de commit clairs et en français

✅ **Si ces règles sont suivies, le projet sera toujours dans un état stable et compréhensible par n'importe quelle instance de développement.**
