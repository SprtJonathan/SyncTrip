# SyncTrip - Règles de Développement

> **Document de référence obligatoire** pour tout développement sur le projet SyncTrip.
> Ces règles doivent être respectées systématiquement pour garantir la qualité, la maintenabilité et la cohérence du code.

---

## Table des matières

1. [Principes Architecturaux](#principes-architecturaux)
2. [Modèle de Données](#modèle-de-données)
3. [Conventions de Code](#conventions-de-code)
4. [Patterns et Pratiques](#patterns-et-pratiques)
5. [Sécurité](#sécurité)
6. [Performance](#performance)
7. [Tests](#tests)
8. [Git et Versioning](#git-et-versioning)

---

## Principes Architecturaux

### Architecture Globale

**RÈGLE 1 : Architecture Client-Serveur Centralisée**
- ✅ TOUJOURS utiliser un serveur central pour la synchronisation
- ❌ JAMAIS implémenter de logique P2P
- **Raison** : Fiabilité, historique centralisé, gestion simplifiée des reconnexions

**RÈGLE 2 : Séparation des Responsabilités**
- ✅ **Frontend MAUI** : UI, ViewModel, cache local uniquement
- ✅ **Backend API** : Logique métier, validation, persistance
- ❌ JAMAIS de logique métier complexe dans le frontend
- ❌ JAMAIS de code UI dans le backend

**RÈGLE 3 : Framework et Versions**
- ✅ .NET 10 OBLIGATOIRE pour frontend ET backend
- ✅ PostgreSQL 16+ pour la base de données
- ✅ Redis 7+ pour le cache
- ❌ JAMAIS utiliser des versions antérieures sans justification documentée

### Architecture N-Tiers (Layered Architecture)

**RÈGLE 4 : Respecter les couches**

```
Présentation (MAUI Views/XAML)
    ↓ appelle uniquement
Application (ViewModels)
    ↓ appelle uniquement
Domaine (Entities, Business Logic)
    ↓ appelle uniquement
Infrastructure (API Client, DB, Services)
```

- ✅ Une couche ne peut appeler QUE la couche directement en dessous
- ❌ JAMAIS de dépendance inverse (ex: Infrastructure → Présentation)
- ❌ JAMAIS de couche sautée (ex: Présentation → Infrastructure direct)

**RÈGLE 5 : Injection de Dépendances**
- ✅ TOUJOURS utiliser l'injection de dépendances (DI)
- ✅ TOUJOURS injecter des interfaces, jamais des classes concrètes
- ❌ JAMAIS utiliser `new` pour instancier des services
- ❌ JAMAIS de singleton manuel (utiliser DI container)

---

## Modèle de Données

### Concepts Clés

**RÈGLE 6 : Hiérarchie Convoy → Trip**

```
Convoy (Groupe persistant)
├── Code alphanumérique permanent (6 chars)
├── Participants (persistent entre trips)
└── Trips (multiples voyages)
    ├── Trip 1 (Finished)
    ├── Trip 2 (Active) ← UN SEUL actif à la fois
    └── Trip 3 (Planned)
```

- ✅ Un **Convoy** peut avoir PLUSIEURS **Trips**
- ✅ UN SEUL Trip peut être `Active` par convoy à la fois
- ✅ Les participants PERSISTENT entre les trips
- ✅ Le code du convoy est PERMANENT et réutilisable
- ❌ JAMAIS créer un Trip sans Convoy parent
- ❌ JAMAIS permettre 2 trips actifs simultanément dans un convoy

**RÈGLE 7 : Destination Obligatoire**
- ✅ CHAQUE Trip DOIT avoir une destination (Waypoint de type Destination)
- ✅ La destination est créée AVANT le Trip
- ❌ JAMAIS créer un Trip sans destination
- ❌ JAMAIS permettre la suppression d'une destination

**RÈGLE 8 : Code Convoy**
- ✅ Format : 6 caractères alphanumériques `[A-Za-z0-9]`
- ✅ Exemples valides : `aB3xK9`, `Zp7mQ2`, `kL9nR4`
- ✅ Unicité vérifiée en base de données
- ✅ Génération via `ConvoyCodeGenerator.GenerateUniqueCodeAsync()`
- ❌ JAMAIS générer manuellement un code
- ❌ JAMAIS utiliser de caractères spéciaux

**RÈGLE 9 : Relations EF Core**
```csharp
// ✅ Correct
Convoy (1) ──→ (N) Trip
Trip (1) ──→ (N) Waypoint
Trip (1) ──→ (1) Waypoint (Destination)
Convoy (1) ──→ (N) ConvoyParticipant
User (1) ──→ (N) ConvoyParticipant

// ❌ Interdit
Waypoint ──X→ Convoy (direct)  // Passer par Trip
```

**RÈGLE 10 : États et Transitions**

**Convoy**
- `Active` → peut créer des trips
- `Archived` → read-only
- Transition : Auto-archivage si vide depuis 30 jours

**Trip**
- `Planned` → `Active` → `Finished`
- `Planned` → `Active` → `Paused` → `Active` → `Finished`
- `Any` → `Cancelled`
- ❌ JAMAIS `Finished` → `Active` (pas de réouverture)

---

## Conventions de Code

### Nommage

**RÈGLE 11 : Conventions C# Standard**

```csharp
// ✅ Correct
public class ConvoyService { }                    // PascalCase pour classes
public interface IConvoyRepository { }            // I + PascalCase pour interfaces
public async Task<Convoy> GetConvoyAsync() { }   // PascalCase pour méthodes
private readonly ILogger _logger;                 // _camelCase pour champs privés
public string ConvoyName { get; set; }           // PascalCase pour propriétés
const int MAX_PARTICIPANTS = 10;                 // UPPER_CASE pour constantes

// ❌ Interdit
public class convoyService { }                    // Mauvaise casse
public class ConvoyServiceImpl { }               // Pas de suffixe "Impl"
public Convoy GetConvoy() { }                    // Manque "Async" si async
private ILogger logger;                          // Manque "_" pour champ privé
```

**RÈGLE 12 : Nommage Spécifique au Domaine**

```csharp
// ✅ Termes corrects à utiliser
Convoy (groupe persistant)
Trip (voyage/instance)
Waypoint (point d'intérêt)
Participant (membre du convoy)
Leader (rôle admin)

// ❌ Termes à éviter
Group → utiliser Convoy
Journey → utiliser Trip
POI → utiliser Waypoint
Member dans le code → utiliser Participant (Member = enum)
Admin → utiliser Leader
```

**RÈGLE 13 : Suffixes de Méthodes**

```csharp
// ✅ Async obligatoire pour méthodes asynchrones
public async Task<Convoy> GetConvoyAsync(Guid id)
public async Task<bool> CreateTripAsync(Trip trip)

// ✅ DTOs et Requests
public class CreateConvoyRequest { }
public class ConvoyDetailResponse { }

// ❌ Interdit
public async Task<Convoy> GetConvoy(Guid id)  // Manque Async
public class ConvoyDto { }  // Trop générique, préciser Request/Response
```

### Structure de Fichiers

**RÈGLE 14 : Organisation des Dossiers**

```
✅ Correct :
Core/
  Entities/User.cs
  Enums/ConvoyStatus.cs
  Interfaces/IConvoyRepository.cs

❌ Interdit :
Core/UserEntity.cs          // Pas de suffixe dans le nom de fichier
Core/IConvoyRepository.cs   // Doit être dans Interfaces/
```

**RÈGLE 15 : Un Fichier = Une Classe Principale**
- ✅ `Convoy.cs` contient uniquement la classe `Convoy`
- ✅ Exceptions : enums liés peuvent être groupés
- ❌ JAMAIS plusieurs entités dans un même fichier

---

## Patterns et Pratiques

### MVVM (Frontend)

**RÈGLE 16 : Pattern MVVM Strict**

```csharp
// ✅ Correct
public partial class ConvoyMapViewModel : BaseViewModel
{
    [ObservableProperty]
    private Convoy _currentConvoy;

    [RelayCommand]
    private async Task CreateTripAsync()
    {
        // Logique ViewModel
        await _convoyService.CreateTripAsync(...);
    }
}

// ❌ Interdit
public class ConvoyMapPage
{
    private void Button_Clicked(object sender, EventArgs e)
    {
        // ❌ Logique métier dans le code-behind
        var convoy = new Convoy { ... };
    }
}
```

- ✅ TOUJOURS utiliser `CommunityToolkit.Mvvm`
- ✅ TOUJOURS `[ObservableProperty]` pour propriétés bindées
- ✅ TOUJOURS `[RelayCommand]` pour commandes
- ❌ JAMAIS de logique métier dans le code-behind (.xaml.cs)
- ❌ JAMAIS de `INotifyPropertyChanged` manuel

**RÈGLE 17 : Binding XAML**

```xml
<!-- ✅ Correct -->
<Label Text="{Binding ConvoyName}" />
<Button Command="{Binding CreateTripCommand}" />

<!-- ❌ Interdit -->
<Button Clicked="Button_Clicked" />  <!-- Pas d'event handler -->
```

### Repository Pattern (Backend)

**RÈGLE 18 : Repository Obligatoire**

```csharp
// ✅ Correct
public interface IConvoyRepository : IRepository<Convoy>
{
    Task<Convoy?> GetByCodeAsync(string code);
    Task<Convoy?> GetWithActiveTripAsync(Guid convoyId);
}

public class ConvoyRepository : Repository<Convoy>, IConvoyRepository
{
    public async Task<Convoy?> GetByCodeAsync(string code)
    {
        return await _context.Convoys
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Code == code);
    }
}

// ❌ Interdit - Accès direct au DbContext dans le controller
public class ConvoysController
{
    public async Task<IActionResult> Get(Guid id)
    {
        var convoy = await _context.Convoys.FindAsync(id);  // ❌
    }
}
```

- ✅ TOUJOURS passer par un repository
- ✅ TOUJOURS définir une interface
- ❌ JAMAIS de `DbContext` injecté dans un controller
- ❌ JAMAIS de `DbContext` dans les ViewModels

**RÈGLE 19 : Queries Complexes**

```csharp
// ✅ Correct - Encapsuler dans le repository
public async Task<Convoy?> GetConvoyWithDetailsAsync(Guid id)
{
    return await _context.Convoys
        .Include(c => c.Participants)
            .ThenInclude(p => p.User)
        .Include(c => c.Trips.Where(t => t.Status == TripStatus.Active))
            .ThenInclude(t => t.Waypoints)
        .AsSplitQuery()  // Optimisation
        .FirstOrDefaultAsync(c => c.Id == id);
}

// ❌ Interdit - Query dans le service/controller
```

### Services

**RÈGLE 20 : Responsabilité des Services**

```csharp
// ✅ Correct - Service métier
public class TripService
{
    public async Task<Trip> CreateTripAsync(CreateTripRequest request)
    {
        // 1. Validation métier
        if (await _tripRepository.HasActiveTripAsync(request.ConvoyId))
            throw new BusinessException("Un trip est déjà actif");

        // 2. Création entités
        var destination = new Waypoint { ... };
        var trip = new Trip { ... };

        // 3. Persistance via repository
        await _waypointRepository.AddAsync(destination);
        await _tripRepository.AddAsync(trip);

        return trip;
    }
}

// ❌ Interdit - Logique métier dans le controller
public class TripsController
{
    public async Task<IActionResult> Create(CreateTripRequest request)
    {
        // ❌ Validation métier ici
        if (await _context.Trips.AnyAsync(...)) { }
    }
}
```

- ✅ Controllers = routing et validation HTTP uniquement
- ✅ Services = logique métier et orchestration
- ✅ Repositories = accès données uniquement
- ❌ JAMAIS de logique métier dans les controllers
- ❌ JAMAIS d'accès direct aux données dans les services

---

## Sécurité

**RÈGLE 21 : Authentification Passwordless Uniquement**

```csharp
// ✅ Correct
public async Task<string> RequestMagicLinkAsync(string email)
{
    var token = Guid.NewGuid().ToString();
    var magicLink = new MagicLinkToken
    {
        Token = token,
        ExpiresAt = DateTime.UtcNow.AddMinutes(15),  // Court !
        IsUsed = false
    };
    // ...
}

// ❌ Interdit
public class User
{
    public string PasswordHash { get; set; }  // ❌ Pas de mot de passe !
}
```

- ✅ Magic Link par email (15 min d'expiration)
- ✅ 2FA optionnelle par téléphone (OTP SMS)
- ✅ JWT avec refresh token (access: 1h, refresh: 30j)
- ❌ JAMAIS de système de mot de passe
- ❌ JAMAIS de hash de mot de passe stocké

**RÈGLE 22 : Validation des Entrées**

```csharp
// ✅ Correct - FluentValidation
public class CreateConvoyRequestValidator : AbstractValidator<CreateConvoyRequest>
{
    public CreateConvoyRequestValidator()
    {
        RuleFor(x => x.ConvoyName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.DestinationLatitude)
            .InclusiveBetween(-90, 90);
    }
}

// ❌ Interdit - Pas de validation ou validation manuelle
public async Task<IActionResult> Create(CreateConvoyRequest request)
{
    if (string.IsNullOrEmpty(request.ConvoyName)) { }  // ❌
}
```

**RÈGLE 23 : Autorisation**

```csharp
// ✅ Correct
[Authorize]
public class ConvoysController : ControllerBase
{
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateConvoyRequest request)
    {
        // Vérifier que l'utilisateur est Leader du convoy
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!await _convoyService.IsLeaderAsync(id, Guid.Parse(userId)))
            return Forbid();
        // ...
    }
}

// ❌ Interdit - Pas de vérification d'autorisation
```

- ✅ TOUJOURS vérifier les permissions avant toute action
- ✅ TOUJOURS utiliser `[Authorize]` sur les controllers
- ❌ JAMAIS faire confiance aux données du client
- ❌ JAMAIS exposer d'IDs sensibles sans vérification

**RÈGLE 24 : Données Sensibles**

```csharp
// ✅ Correct
public class ConvoyDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    // ❌ Ne PAS exposer le code si pas leader
}

// Configuration logging
_logger.LogInformation("User {UserId} joined convoy {ConvoyId}",
    userId.ToString()[..8],  // ✅ Tronquer les IDs
    convoyId.ToString()[..8]);

// ❌ Interdit
_logger.LogInformation("GPS: {Lat}, {Lon}", lat, lon);  // ❌ Pas de position GPS dans les logs
```

- ✅ JAMAIS logger de positions GPS
- ✅ JAMAIS logger d'emails complets
- ✅ JAMAIS exposer de tokens dans les logs
- ✅ Tronquer les IDs dans les logs

---

## Performance

**RÈGLE 25 : Optimisation GPS**

```csharp
// ✅ Correct
private const int UPDATE_INTERVAL_SECONDS = 5;
private const double MIN_DISTANCE_METERS = 10;

public async Task<Location> GetCurrentLocationAsync()
{
    var location = await Geolocation.GetLocationAsync(...);

    // N'envoyer que si déplacement significatif
    if (_lastLocation == null ||
        location.CalculateDistance(_lastLocation) > MIN_DISTANCE_METERS)
    {
        await SyncLocationAsync(location);
        _lastLocation = location;
    }

    return location;
}

// ❌ Interdit - Envoyer à chaque update sans filtre
```

**RÈGLE 26 : Cache et TTL**

```csharp
// ✅ Correct - Redis avec TTL
await _cache.SetStringAsync(
    $"position:{tripId}:{userId}",
    JsonSerializer.Serialize(location),
    new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)  // TTL court
    });

// ❌ Interdit - Pas de TTL ou TTL trop long
await _cache.SetStringAsync(key, value);  // ❌ Pas d'expiration
```

**RÈGLE 27 : Queries EF Core**

```csharp
// ✅ Correct
var convoy = await _context.Convoys
    .AsNoTracking()  // Si read-only
    .AsSplitQuery()  // Pour includes multiples
    .Include(c => c.Participants)
    .Include(c => c.Trips.Where(t => t.Status == TripStatus.Active))
    .FirstOrDefaultAsync(c => c.Id == id);

// ❌ Interdit
var convoy = await _context.Convoys.ToListAsync();  // ❌ Tout charger
var trips = convoy.Trips.ToList();  // ❌ N+1 queries
```

- ✅ TOUJOURS utiliser `AsNoTracking()` pour les requêtes read-only
- ✅ TOUJOURS utiliser `AsSplitQuery()` pour les includes complexes
- ✅ TOUJOURS filtrer avec `Where()` dans les includes
- ❌ JAMAIS de `ToList()` puis filtrer en mémoire

**RÈGLE 28 : SignalR Batching**

```csharp
// ✅ Correct - Limiter la fréquence
private DateTime _lastBroadcast = DateTime.MinValue;
private const int MIN_BROADCAST_INTERVAL_MS = 1000;

public async Task BroadcastLocationAsync(LocationUpdate update)
{
    var now = DateTime.UtcNow;
    if ((now - _lastBroadcast).TotalMilliseconds < MIN_BROADCAST_INTERVAL_MS)
        return;  // Throttle

    await Clients.Group(update.TripId.ToString())
        .SendAsync("LocationUpdate", update);

    _lastBroadcast = now;
}

// ❌ Interdit - Broadcast sans limite
```

---

## Tests

**RÈGLE 29 : Couverture de Tests**

- ✅ MINIMUM 70% de couverture pour le code métier
- ✅ 100% de couverture pour les services critiques (Auth, Payment futur)
- ✅ Tests unitaires pour toute logique métier
- ✅ Tests d'intégration pour les APIs
- ❌ JAMAIS commit sans tests pour nouvelle feature

**RÈGLE 30 : Structure des Tests**

```csharp
// ✅ Correct - AAA Pattern
[Fact]
public async Task CreateTrip_WhenActiveTripExists_ShouldThrowException()
{
    // Arrange
    var convoy = new Convoy { Id = Guid.NewGuid() };
    _tripRepositoryMock
        .Setup(x => x.HasActiveTripAsync(convoy.Id))
        .ReturnsAsync(true);

    // Act
    var act = async () => await _tripService.CreateTripAsync(convoy.Id, ...);

    // Assert
    await act.Should().ThrowAsync<BusinessException>()
        .WithMessage("Un trip est déjà actif");
}

// ❌ Interdit - Test obscur sans AAA
[Fact]
public async Task Test1()
{
    var result = await _service.DoSomething();
    Assert.True(result);  // ❌ Qu'est-ce qu'on teste ?
}
```

**RÈGLE 31 : Nommage des Tests**

```csharp
// ✅ Correct
MethodName_StateUnderTest_ExpectedBehavior

public async Task CreateTrip_WhenNoDestination_ShouldThrowValidationException()
public async Task GetConvoy_WithValidCode_ShouldReturnConvoy()

// ❌ Interdit
public void Test1()
public void TestCreateTrip()
```

---

## Git et Versioning

**RÈGLE 32 : Messages de Commit**

```bash
# ✅ Correct - Conventional Commits
feat(convoy): add multi-trip support for escale scenarios
fix(auth): correct magic link expiration validation
docs(readme): update installation instructions for .NET 10
refactor(trip): extract destination creation to separate method

# ❌ Interdit
git commit -m "fix bug"
git commit -m "changes"
git commit -m "wip"
```

Format : `type(scope): description`
- `feat`: nouvelle fonctionnalité
- `fix`: correction de bug
- `refactor`: refactoring sans changement de comportement
- `docs`: documentation
- `test`: ajout/modification de tests
- `chore`: tâches de maintenance

**RÈGLE 33 : Branches**

```bash
# ✅ Correct
main                           # Production
develop                        # Développement
feature/multi-trip-support     # Nouvelle feature
fix/magic-link-expiration      # Bug fix
refactor/trip-creation         # Refactoring

# ❌ Interdit
my-branch
test
new-stuff
```

**RÈGLE 34 : Pull Requests**

- ✅ TOUJOURS créer une PR pour merger dans develop/main
- ✅ TOUJOURS avoir au moins 1 review
- ✅ TOUJOURS que les tests passent (CI)
- ✅ TOUJOURS squash les commits avant merge
- ❌ JAMAIS de merge direct dans main
- ❌ JAMAIS de PR avec des tests échouants

---

## Règles Spécifiques au Projet

**RÈGLE 35 : Contraintes Métier**

```csharp
// ✅ TOUJOURS vérifier ces contraintes

// 1. UN SEUL trip actif par convoy
private async Task<bool> ValidateSingleActiveTripAsync(Guid convoyId)
{
    var activeCount = await _context.Trips
        .CountAsync(t => t.ConvoyId == convoyId && t.Status == TripStatus.Active);

    return activeCount <= 1;
}

// 2. Destination obligatoire pour chaque trip
private void ValidateTripHasDestination(Trip trip)
{
    if (trip.DestinationWaypointId == Guid.Empty)
        throw new BusinessException("Destination is required");
}

// 3. Code convoy unique
private async Task<bool> ValidateConvoyCodeUniqueAsync(string code)
{
    return !await _context.Convoys.AnyAsync(c => c.Code == code);
}
```

**RÈGLE 36 : États Valides**

```csharp
// ✅ Transitions autorisées
Trip:
  Planned → Active ✓
  Planned → Cancelled ✓
  Active → Paused ✓
  Active → Finished ✓
  Active → Cancelled ✓
  Paused → Active ✓
  Paused → Cancelled ✓

// ❌ Transitions interdites
Trip:
  Finished → Active ✗
  Finished → Paused ✗
  Cancelled → Active ✗
```

**RÈGLE 37 : Nettoyage Automatique**

```csharp
// ✅ Background service obligatoire
public class ConvoyCleanupService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Archiver convoys vides depuis 30 jours
            // Supprimer convoys archivés depuis 1 an

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}
```

- ✅ Exécution toutes les 24h
- ✅ Archivage à 30 jours sans participants
- ✅ Suppression à 1 an après archivage
- ❌ JAMAIS de suppression immédiate

---

## Checklist avant Commit

Avant CHAQUE commit, vérifier :

- [ ] Le code compile sans warning
- [ ] Les tests unitaires passent
- [ ] Pas de TODO/FIXME laissés
- [ ] Pas de `Console.WriteLine` ou logs de debug
- [ ] Pas de code commenté (supprimer ou documenter pourquoi)
- [ ] Les conventions de nommage sont respectées
- [ ] Les principes SOLID sont appliqués
- [ ] L'injection de dépendances est utilisée
- [ ] Les erreurs sont gérées (try/catch où nécessaire)
- [ ] Les ressources sont libérées (using, dispose)
- [ ] La documentation XML est présente sur les méthodes publiques
- [ ] Le message de commit suit le format Conventional Commits

---

## Exceptions aux Règles

Les règles peuvent être enfreintes UNIQUEMENT si :

1. **Justification documentée** dans un commentaire de code
2. **Validation par le lead technique**
3. **Alternative pire** que l'infraction

Exemple :

```csharp
// EXCEPTION à la RÈGLE 18 (Repository Pattern)
// Justification : Query ultra-complexe spécifique à ce cas uniquement
// Validé par : Lead Tech le 2025-11-16
// Alternative : Créer une méthode repository complexe pour 1 seul usage
var result = await _context.Trips
    .FromSqlRaw("COMPLEX RAW SQL...")
    .ToListAsync();
```

---

## Ressources

- [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [Clean Code](https://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882)
- [Conventional Commits](https://www.conventionalcommits.org/)

---

**Version** : 1.0
**Date** : 2025-11-16
**Statut** : En vigueur

**Ces règles sont OBLIGATOIRES pour tout contributeur au projet SyncTrip.**
