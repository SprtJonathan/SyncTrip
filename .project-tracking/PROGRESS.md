# SyncTrip - Suivi de Progression

**Dernière mise à jour** : 13 Février 2026
**Statut Global** : 6/6 Features COMPLÈTES (Backend + Mobile AvaloniaUI) + Backend Geocoding/Routing
**Migration MAUI → AvaloniaUI** : TERMINÉE (11 étapes, build 0 erreurs, tests 320/320)

---

## Vue d'ensemble

Le projet SyncTrip est développé en utilisant une **approche verticale (vertical slice)**.
Chaque feature est développée de bout en bout (Core → Application → Infrastructure → API → Mobile) avant de passer à la suivante.

---

## Progression par Feature

### ✅ TERMINÉ

#### Feature 1 : Authentification Magic Link
**Statut** : TERMINÉ (Backend + Mobile)
**Progression** : 100%

**Composants terminés** :
- [x] Core : Entités User, MagicLinkToken
- [x] Core : Interfaces IUserRepository, IAuthService, IEmailService
- [x] Shared : DTOs Auth (MagicLinkRequest, VerifyTokenRequest, CompleteRegistrationRequest, VerifyTokenResponse)
- [x] Application : Commands Auth (SendMagicLink, VerifyToken, CompleteRegistration)
- [x] Application : Validators Auth (CompleteRegistrationValidator)
- [x] Infrastructure : Repositories (UserRepository, MagicLinkTokenRepository)
- [x] Infrastructure : Services (AuthService, EmailService)
- [x] Infrastructure : Configuration EF Core + Migration initiale
- [x] API : AuthController avec 3 endpoints (SendMagicLink, VerifyToken, CompleteRegistration)
- [x] API : Configuration Program.cs (MediatR, JWT, FluentValidation, CORS, Swagger)
- [x] Tests : Tests unitaires entités User (17 tests - validation âge > 14 ans)
- [x] Tests : Tests handlers CompleteRegistrationCommandHandler (5 tests)
- [x] Vérification : Compilation sans erreur de l'API
- [x] Vérification : Tous les tests passent (22/22)

**Composants Mobile ajoutés** :
- [x] Mobile : Core/Services (IApiService, ApiService, IAuthenticationService, AuthenticationService)
- [x] Mobile : Core/Converters (InvertedBoolConverter, IsNotNullOrEmptyConverter, SuccessErrorColorConverter)
- [x] Mobile : Features/Authentication/ViewModels (MagicLinkViewModel, RegistrationViewModel)
- [x] Mobile : Features/Authentication/Views (MagicLinkPage, RegistrationPage)
- [x] Mobile : Configuration MauiProgram.cs (DI HttpClient, Services, ViewModels, Pages)
- [x] Mobile : Configuration AppShell.xaml (Routes navigation)
- [x] Mobile : Styles (ajout couleurs Success, Error, Warning)
- [x] Vérification : Compilation complète réussie (toutes plateformes : Android, iOS, MacCatalyst, Windows)

#### Feature 2 : Profil & Garage
**Statut** : TERMINÉ (Backend + Mobile + Tests)
**Progression** : 100%

**Composants terminés** :
- [x] Core : Enums (LicenseType, VehicleType)
- [x] Core : Entités (Brand, Vehicle, UserLicense)
- [x] Core : Relations User (Vehicles, Licenses)
- [x] Core : Interfaces (IVehicleRepository, IBrandRepository)
- [x] Shared : DTOs Users (UserProfileDto, UpdateUserProfileRequest)
- [x] Shared : DTOs Vehicles (VehicleDto, CreateVehicleRequest, UpdateVehicleRequest)
- [x] Shared : DTOs Brands (BrandDto)
- [x] Application : Queries (GetUserProfile, GetUserVehicles, GetBrands)
- [x] Application : Commands (UpdateUserProfile, CreateVehicle, UpdateVehicle, DeleteVehicle)
- [x] Application : Validators FluentValidation (UpdateUserProfile, CreateVehicle, UpdateVehicle)
- [x] Infrastructure : Repositories (VehicleRepository, BrandRepository)
- [x] Infrastructure : Configurations EF Core (Brand, Vehicle, UserLicense)
- [x] Infrastructure : Seed data 40 marques de véhicules (motos, voitures, utilitaires)
- [x] Infrastructure : Migration EF Core appliquée
- [x] Infrastructure : DependencyInjection.cs mis à jour
- [x] API : UsersController (GET/PUT /users/me)
- [x] API : VehiclesController (CRUD complet)
- [x] API : BrandsController (GET /brands)
- [x] Tests : Entités Vehicle, Brand, UserLicense (96 tests)
- [x] Tests : Handlers (CreateVehicle, GetUserProfile, UpdateUserProfile - 55 tests)
- [x] Vérification : Compilation sans erreur Backend
- [x] Vérification : Tous les tests passent (151/151)

**Composants Mobile ajoutés** :
- [x] Mobile : Services (IUserService, UserService, IVehicleService, VehicleService, IBrandService, BrandService)
- [x] Mobile : ViewModels (ProfileViewModel, GarageViewModel, AddVehicleViewModel)
- [x] Mobile : Converters (VehicleTypeConverter, IsNotNullConverter)
- [x] Mobile : Views (ProfilePage.xaml, GaragePage.xaml, AddVehiclePage.xaml)
- [x] Mobile : Configuration MauiProgram.cs (Services, ViewModels, Pages)
- [x] Mobile : Configuration AppShell.xaml (Onglets Profile et Garage, route addvehicle)
- [x] Vérification : Configuration complète DI et navigation

#### Feature 3 : Convois
**Statut** : TERMINÉ (Backend + Mobile + Tests)
**Progression** : 100%

**Composants terminés** :
- [x] Core : Enum ConvoyRole (Member, Leader)
- [x] Core : Entité Convoy (factory method, JoinCode 6 chars crypto-sécurisé, gestion membres)
- [x] Core : Entité ConvoyMember (composite PK, rôles Leader/Member)
- [x] Core : Interface IConvoyRepository
- [x] Shared : DTOs Convoys (CreateConvoyRequest, JoinConvoyRequest, ConvoyDto, ConvoyMemberDto, ConvoyDetailsDto)
- [x] Application : Commands (CreateConvoy, JoinConvoy, LeaveConvoy, KickMember, TransferLeadership, DissolveConvoy)
- [x] Application : Queries (GetConvoyByCode, GetUserConvoys)
- [x] Application : Validators FluentValidation (CreateConvoy, JoinConvoy)
- [x] Infrastructure : ConvoyRepository avec Include chains (Members → User, Vehicle → Brand)
- [x] Infrastructure : Configurations EF Core (Convoy, ConvoyMember) avec index unique JoinCode
- [x] Infrastructure : Migration EF Core AddConvoyFeature
- [x] Infrastructure : DependencyInjection.cs mis à jour
- [x] API : ConvoysController avec 7 endpoints [Authorize]
  - POST /api/convoys (créer)
  - GET /api/convoys/{code} (détails)
  - GET /api/convoys/my (mes convois)
  - POST /api/convoys/{code}/join (rejoindre)
  - POST /api/convoys/{code}/leave (quitter)
  - POST /api/convoys/{code}/kick/{targetUserId} (exclure)
  - POST /api/convoys/{code}/transfer/{newLeaderUserId} (transférer leadership)
  - DELETE /api/convoys/{code} (dissoudre)
- [x] Tests : ConvoyTests (22 tests) + ConvoyMemberTests (6 tests)
- [x] Tests : CreateConvoyCommandHandlerTests (5 tests) + JoinConvoyCommandHandlerTests (4 tests)
- [x] Vérification : Compilation sans erreur Backend + Mobile
- [x] Vérification : Tous les tests passent (188/188)

**Composants Mobile ajoutés** :
- [x] Mobile : Services (IConvoyService, ConvoyService)
- [x] Mobile : ViewModels (ConvoyLobbyViewModel, CreateConvoyViewModel, JoinConvoyViewModel)
- [x] Mobile : Views (ConvoyLobbyPage, CreateConvoyPage, JoinConvoyPage)
- [x] Mobile : Configuration MauiProgram.cs (ConvoyService, ViewModels, Pages)
- [x] Mobile : Configuration AppShell.xaml (onglet Convois) + routes navigation

#### Sécurisation Production (P0 - Critical)
**Statut** : TERMINÉ
**Date** : 28 Novembre 2025
**Progression** : 100%

**Contexte** :
Audit de sécurité complet réalisé avec l'agent dotnet-maui-expert. Identification et résolution de 5 problèmes critiques (P0) bloquants pour la production.

**Composants sécurisés** :
- [x] **.gitignore** : Création fichier complet .NET/MAUI pour prévenir commit de secrets
  - Exclusion appsettings.*.json (sauf appsettings.json)
  - Exclusion secrets.json, certificats *.pfx/*.p12
  - Exclusion base de données locale, logs, binaires
  - Protection bin/, obj/, .vs/, .idea/

- [x] **User Secrets** : Configuration stockage sécurisé des secrets en développement
  - ConnectionStrings:DefaultConnection (PostgreSQL)
  - JwtSettings:SecretKey
  - EmailSettings:SmtpUser et SmtpPassword
  - Commande : `dotnet user-secrets set "Key" "Value"`

- [x] **appsettings.json** : Nettoyage des secrets en clair
  - Remplacement ConnectionString par placeholder
  - Remplacement JwtSettings:SecretKey par placeholder
  - Remplacement EmailSettings (SmtpUser, SmtpPassword) par placeholders
  - Message explicite : "SET_VIA_USER_SECRETS_OR_ENVIRONMENT_VARIABLES"

- [x] **Global Error Handling** : Middleware gestion d'erreurs Production
  - `Program.cs` : UseExceptionHandler avec réponse générique
  - Pas d'exposition de stack traces en production
  - Logging complet des erreurs avec TraceId
  - Réponse JSON standardisée avec message utilisateur + TraceId
  - HSTS activé en production

- [x] **Rate Limiting** : Protection contre brute force et abus API
  - Rate limiter global : 100 requêtes/minute par IP
  - Rate limiter spécifique auth : 5 requêtes/10 minutes par IP
  - Middleware UseRateLimiter activé
  - Attribut [EnableRateLimiting("auth")] sur AuthController
  - Réponse HTTP 429 avec RetryAfter en cas de dépassement

- [x] **EF Core Warnings** : Suppression du masquage de warnings
  - ApplicationDbContext : Retrait ConfigureWarnings
  - Détection proactive des changements de schéma
  - Meilleure visibilité des migrations pendantes

- [x] **Documentation** : Mise à jour ARCHITECTURE.md
  - Version 1.1 (28 Novembre 2025)
  - Mention explicite ".NET 10 LTS (Long Term Support)"
  - Confirmation version stable pour production

**Tests** :
- [x] Compilation Backend sans erreur
- [x] Tous les tests passent (151/151 - 100%)
- [x] Vérification User Secrets fonctionnels

**Commit** :
- `1152142` - Security hardening for production readiness

---

#### Correctifs Mobile — Fondations
**Statut** : TERMINÉ
**Date** : 12 Février 2026
**Priorité** : CRITIQUE (bloquant pour toutes les features Mobile)

**Corrections appliquées** :
- [x] Restructuration AppShell : routes login/main séparées (ShellContent + TabBar)
- [x] App.xaml.cs : vérification `IsAuthenticatedAsync()` au démarrage → redirect vers main si authentifié
- [x] AuthorizationMessageHandler : DelegatingHandler qui injecte le JWT Bearer automatiquement
- [x] AddVehiclePage.xaml.cs créé (code-behind avec LoadBrands au OnAppearing)
- [x] Icônes SVG ajoutées : person.svg, car.svg, convoy.svg dans Resources/Images
- [x] Typos accents français corrigés dans CreateConvoyPage.xaml et JoinConvoyPage.xaml
- [x] Route de déconnexion corrigée dans ProfileViewModel (`"//login"`)
- [x] MainPage.xaml/MainPage.xaml.cs inutilisés supprimés

---

#### Feature 4 : Navigation GPS
**Statut** : TERMINÉ (Backend + Mobile)
**Progression** : 100%

**Composants Backend terminés** :
- [x] Core : Enums (TripStatus, RouteProfile, WaypointType)
- [x] Core : Entités Trip, TripWaypoint (factory methods, domain validation)
- [x] Core : Modification Convoy (ajout collection Trips)
- [x] Core : Interface ITripRepository
- [x] Shared : 6 DTOs Trips (StartTripRequest, CreateWaypointRequest, AddWaypointRequest, TripDto, TripDetailsDto, TripWaypointDto)
- [x] Application : 4 Commands (StartTrip, EndTrip, AddWaypoint, RemoveWaypoint)
- [x] Application : 3 Queries (GetTripById, GetActiveTripByConvoyId, GetConvoyTrips)
- [x] Application : 2 Validators (StartTripValidator, AddWaypointValidator)
- [x] Infrastructure : 2 EF Configurations (TripConfiguration, TripWaypointConfiguration)
- [x] Infrastructure : TripRepository avec Include chains
- [x] Infrastructure : DI registration + Migration AddTripFeature
- [x] API : TripsController (7 endpoints REST nested sous convoys)
- [x] API : TripHub SignalR (positions temps réel éphémères)
- [x] API : Program.cs (AddSignalR, JWT query string, MapHub)
- [x] Tests Core : 27 tests (Trip 14, TripWaypoint 13)
- [x] Tests Application : 15 tests (StartTrip 5, EndTrip 4, AddWaypoint 4, GetTripById 2)

**Composants Mobile ajoutés** :
- [x] Mobile : Services (IApiService.DeleteAsync, ITripService, TripService)
- [x] Mobile : Services (ISignalRService, SignalRService — HubConnection, JWT, ReceiveLocationUpdate)
- [x] Mobile : Converters (TripStatusConverter, ConvoyRoleConverter)
- [x] Mobile : ConvoyDetailPage (VM + XAML — détails convoi, membres, voyage actif, démarrer/terminer)
- [x] Mobile : CockpitPage (VM + XAML — carte Mapsui, MyLocationLayer, WritableLayer membres)
- [x] Mobile : Navigation ConvoyLobby → ConvoyDetail → CockpitPage (TapGestureRecognizer, routes Shell)
- [x] Mobile : Configuration MauiProgram.cs (UseSkiaSharp, TripService, SignalRService, VMs, Pages)
- [x] Mobile : Configuration AppShell.xaml.cs (routes convoydetail, cockpit)

---

#### Feature 5 : Système de Vote
**Statut** : TERMINÉ (Backend + Mobile)
**Progression** : 100%

**Composants Backend terminés** :
- [x] Core : Enums (StopType, ProposalStatus)
- [x] Core : Entités StopProposal (factory, CastVote, Resolve règle du silence, AllMembersVoted, SetCreatedWaypoint)
- [x] Core : Entité Vote (factory method)
- [x] Core : Interface IStopProposalRepository
- [x] Shared : 4 DTOs Voting (ProposeStopRequest, CastVoteRequest, StopProposalDto, VoteDto)
- [x] Application : 2 Commands (ProposeStop, CastVote) avec auto-vote YES et résolution anticipée
- [x] Application : 2 Queries (GetActiveProposal, GetProposalHistory)
- [x] Application : 2 Validators FluentValidation (ProposeStop, CastVote)
- [x] Application : Interface ITripNotificationService (abstraction SignalR)
- [x] Infrastructure : StopProposalRepository avec Include chains
- [x] Infrastructure : 2 Configurations EF Core (StopProposal, Vote) avec index composites
- [x] Infrastructure : ProposalResolutionService (BackgroundService, poll 5s, résout propositions expirées)
- [x] Infrastructure : DI registration + Migration AddVotingSystem
- [x] API : VotingController (4 endpoints REST nested sous convoys/trips/proposals)
  - POST `.../proposals` (proposer arrêt → 201)
  - GET `.../proposals/active` (proposition active → 200/404)
  - GET `.../proposals` (historique → 200)
  - POST `.../proposals/{proposalId}/vote` (voter → 200/400/404)
- [x] API : TripNotificationService (implémentation ITripNotificationService via IHubContext<TripHub>)
- [x] API : Program.cs (registration ITripNotificationService)
- [x] Tests Core : 34 tests (StopProposal 28, Vote 6)
- [x] Tests Application : 19 tests (ProposeStop 7, CastVote 8, GetActiveProposal 2, GetProposalHistory 2)

**Composants Mobile ajoutés** :
- [x] App : IVotingService / VotingService (REST: proposer, voter, historique)
- [x] App : SignalR events StopProposed, VoteUpdate, ProposalResolved sur TripHub
- [x] App : VotingViewModel (propose stop, cast vote, countdown, SignalR realtime)
- [x] App : VotingView.axaml (carte proposition, vote OUI/NON, compteur, résultats)
- [x] App : StopTypeConverter, ProposalStatusConverter
- [x] App : Navigation CockpitView → VotingView (bouton "Proposer un arrêt")

---

#### Feature 6 : Chat
**Statut** : TERMINÉ (Backend + Mobile)
**Progression** : 100%

**Composants Backend terminés** :
- [x] Core : Entité Message (factory Create, validation contenu max 500 chars)
- [x] Core : Interface IMessageRepository (GetByConvoyIdAsync avec pagination curseur, AddAsync)
- [x] Shared : 2 DTOs Chat (SendMessageRequest record, MessageDto class avec SenderUsername/AvatarUrl)
- [x] Application : 1 Command (SendMessage) — crée message + persiste + notifie SignalR
- [x] Application : 1 Query (GetConvoyMessages) — pagination curseur (before + pageSize)
- [x] Application : 1 Validator FluentValidation (SendMessageValidator)
- [x] Application : Interface IConvoyNotificationService (abstraction SignalR dans Application)
- [x] Infrastructure : MessageRepository avec Include Sender
- [x] Infrastructure : MessageConfiguration EF Core (FK Convoy Cascade, FK User Restrict, index ConvoyId+SentAt DESC)
- [x] Infrastructure : DI registration IMessageRepository → MessageRepository
- [x] API : ConvoyHub SignalR `/hubs/convoy` (JoinConvoy, LeaveConvoy)
- [x] API : MessagesController (2 endpoints REST nested sous `/api/convoys/{convoyId}/messages`)
  - POST `/` (envoyer message → 201)
  - GET `/?pageSize=50&before=...` (historique paginé → 200)
- [x] API : ConvoyNotificationService (implémentation IConvoyNotificationService via IHubContext<ConvoyHub>)
- [x] API : Program.cs (registration IConvoyNotificationService, MapHub ConvoyHub)
- [x] Tests Core : 7 tests (Create valid, 500 chars OK, empty convoyId, empty senderId, empty content, whitespace, >500 chars)
- [x] Tests Application : 9 tests (SendMessage 5, GetConvoyMessages 4)

**Composants Mobile ajoutés** :
- [x] App : IChatService / ChatService (REST: envoyer message, historique paginé curseur)
- [x] App : IConvoySignalRService / ConvoySignalRService (ConvoyHub: ReceiveMessage)
- [x] App : ChatViewModel (messages temps réel, envoi, chargement historique, cleanup)
- [x] App : ChatView.axaml (liste messages, input, envoi, header, chargement plus)
- [x] App : Navigation ConvoyDetailView → ChatView (bouton "Ouvrir le chat")

---

## Métriques

**Features Terminées** : 6 / 6 (Auth + Profil/Garage + Convois + Navigation GPS + Vote + Chat)
**Migration MAUI → AvaloniaUI** : ✅ TERMINÉE (11 étapes)
**Sécurité Production** : ✅ P0 Critical Issues Résolus (5/5)
**Progression Globale** : 100% (6/6 features complètes)
**Dernière compilation** : 13 Fév 2026 - Succès (Backend + App Desktop + Tests)
**Tests Passing** : 320 / 320 (100%)
  - Core.Tests : 202 tests (User, Vehicle, Brand, UserLicense, Convoy, ConvoyMember, Trip, TripWaypoint, TripRoute, StopProposal, Vote, Message)
  - Application.Tests : 118 tests (Auth, Users, Vehicles, Convoys, Trips, Navigation, Voting, Chat)
**Qualité Code** : ✅ Conforme aux spécifications (Clean Architecture, DDD, MVVM)
**Sécurité** : ✅ Production Ready (Rate Limiting, Error Handling, Secrets Management)
**Stack** : .NET 10 LTS — AvaloniaUI 11.3.1 + Mapsui.Avalonia 5.0.0
**Seed Data** : 40 marques de véhicules (motos, voitures, utilitaires)

---

## Historique des Commits

### Session du 23 Novembre 2025

#### Backend (Matin - 17h20)
1. **ea74d52** - `feat(api): ajoute AuthController avec 3 endpoints Magic Link`
2. **1b75d1a** - `feat(api): finalise configuration Program.cs avec MediatR, JWT, FluentValidation et CORS`
3. **acb877b** - `test(core): ajoute tests validation âge pour entité User`
4. **ddeea0f** - `test(application): ajoute tests handlers authentification CompleteRegistration`
5. **1eb3d58** - `chore: ajoute projets de tests à la solution`

#### Mobile (Soir - 21h30)
6. **48911f8** - `feat(mobile): ajoute ApiService pour communication avec API`
   - IApiService et ApiService avec méthodes typées (PostAsync, GetAsync)
7. **93917bd** - `feat(mobile): ajoute AuthenticationService avec gestion JWT et SecureStorage`
   - IAuthenticationService et AuthenticationService avec gestion token sécurisée
8. **a61efd6** - `feat(mobile): ajoute MagicLinkViewModel avec validation email`
   - ViewModel MVVM avec validation format email
9. **742ee7a** - `feat(mobile): ajoute RegistrationViewModel avec validation âge > 14 ans`
   - ViewModel avec validation client-side âge > 14 ans
10. **6709360** - `feat(mobile): ajoute value converters pour bindings XAML`
    - InvertedBoolConverter, IsNotNullOrEmptyConverter, SuccessErrorColorConverter
11. **b484693** - `feat(mobile): ajoute couleurs Success, Error, Warning dans Colors.xaml`
12. **c009032** - `feat(mobile): ajoute MagicLinkPage avec UI Material Design`
    - Page XAML + code-behind pour envoi Magic Link
13. **d3fdd4d** - `feat(mobile): ajoute RegistrationPage avec validation formulaire`
    - Page XAML + code-behind pour inscription utilisateur
14. **4b1fcb8** - `feat(mobile): configure DI dans MauiProgram pour Auth`
    - Configuration HttpClient, Services, ViewModels, Pages
15. **ed82197** - `feat(mobile): configure routes de navigation Auth dans AppShell`
    - Enregistrement routes "magic-link" et "registration"
16. **59d9bcc** - `feat(mobile): ajoute package Microsoft.Extensions.Http pour AddHttpClient`

**Total commits session du 23 Nov** : 16 commits (5 Backend + 11 Mobile)

### Session du 24 Novembre 2025

#### Validation et Corrections (Matin - 14h00)
17. **8a53112** - `fix(tests): corrige erreurs Moq avec paramètres optionnels dans GenerateJwtToken`
    - Erreurs de compilation liées aux expression trees Moq avec paramètres optionnels
    - Ajout explicite du paramètre `additionalClaims` dans Setup et Verify
18. **413f07d** - `chore: supprime fichiers template inutilisés Class1.cs`
    - Nettoyage des fichiers Class1.cs générés par les templates dans Application et Infrastructure

**Validation effectuée** :
- ✅ Compilation complète sans erreur (Backend : Core, Shared, Application, Infrastructure, API)
- ✅ Tous les tests passent (22/22 - 100%)
- ✅ Structure du code conforme aux spécifications ARCHITECTURE.md
- ✅ Entités respectent Clean Architecture (private setters, factory methods)
- ✅ Validation métier âge > 14 ans fonctionnelle
- ✅ ViewModels Mobile suivent MVVM avec CommunityToolkit.Mvvm
- ✅ Services et DI correctement configurés

**Total commits** : 18 commits au total

### Session du 28 Novembre 2025

#### Sécurisation Production (Matin - 10h30)
19. **97c98bf** - `feat(mobile): ajoute ViewModels MVVM pour Feature 2`
20. **10f1eaf** - `docs: met à jour PROGRESS.md avec Feature 2 complète`
21. **6c2dd4f** - `feat(infrastructure): add EF Core migration for Profile & Garage feature`
22. **1152142** - `security: hardening for production readiness`
    - Création .gitignore complet .NET/MAUI
    - Configuration User Secrets pour développement
    - Sécurisation appsettings.json (retrait secrets en clair)
    - Ajout middleware global error handling (production)
    - Implémentation rate limiting (global + auth-specific)
    - Retrait suppression warnings EF Core
    - Mise à jour ARCHITECTURE.md vers v1.1 (.NET 10 LTS)
23. **En cours** - `docs: update PROGRESS.md with security hardening`

**Validation effectuée** :
- ✅ Audit de sécurité complet avec dotnet-maui-expert
- ✅ Résolution 5 problèmes critiques P0
- ✅ Tous les tests passent (151/151 - 100%)
- ✅ Configuration User Secrets fonctionnelle
- ✅ Rate Limiting opérationnel
- ✅ Error Handling production-ready

**Total commits session du 28 Nov** : 5 commits (Sécurité + Documentation)

### Session du 9 Février 2026

#### Nettoyage Git + Feature 3 : Convois
24. **a34a1fc** - `chore: nettoie l'index git et complète le .gitignore`
    - Suppression 7669 fichiers trackés inutiles (bin/, obj/, .idea/, .claude/)
    - Ajout `.claude/` au .gitignore
25. **360f812** - `feat(core): ajoute entités Convoy, ConvoyMember et IConvoyRepository`
    - Enum ConvoyRole (Member, Leader)
    - Entité Convoy avec factory method, JoinCode crypto-sécurisé (6 chars, sans caractères ambigus)
    - Entité ConvoyMember avec PK composite, gestion rôles
    - Interface IConvoyRepository
26. **8435bd7** - `feat(shared): ajoute DTOs pour les Convois`
    - CreateConvoyRequest, JoinConvoyRequest
    - ConvoyDto, ConvoyMemberDto, ConvoyDetailsDto
27. **c4f4b98** - `feat(application): ajoute commands, queries et validators pour les Convois`
    - 6 Commands (Create, Join, Leave, Kick, Transfer, Dissolve)
    - 2 Queries (GetByCode, GetUserConvoys)
    - 2 Validators FluentValidation
28. **e1342c9** - `feat(infrastructure): ajoute repository, configurations EF Core et migration Convoys`
    - ConvoyRepository avec Include chains
    - Configurations EF Core (index unique JoinCode, PK composite ConvoyMembers)
    - Migration AddConvoyFeature
29. **a8ef1af** - `feat(api): ajoute ConvoysController avec 7 endpoints`
    - 7 endpoints REST [Authorize] pour gestion complète des convois
30. **5260b0d** - `test: ajoute tests unitaires pour les Convois (37 tests)`
    - ConvoyTests (22), ConvoyMemberTests (6), Handlers (9)
31. **7dace97** - `feat(mobile): ajoute pages Convoy (Lobby, Create, Join)`
    - Services, ViewModels MVVM, Pages XAML
    - Configuration DI et navigation Shell

**Validation effectuée** :
- ✅ Compilation sans erreur (API, Mobile Windows, Tests)
- ✅ Tous les tests passent (188/188 - 100%)
- ✅ Structure conforme Clean Architecture + CQRS
- ✅ JoinCode cryptographiquement sécurisé (RandomNumberGenerator)
- ✅ Domaine riche (validation métier dans les entités)

**Total commits session du 9 Fév** : 8 commits (1 nettoyage + 7 Feature 3)

32. **b8c5635** - `fix(mobile): remplace DisplayAlert par DisplayAlertAsync (.NET 10)`
33. **ee48665** - `docs: met à jour PROGRESS.md avec Feature 3 complète`

#### Configuration Docker & API
34. **a1bc96e** - `feat(infra): ajoute Docker, Scalar et configuration API pour dev local`
    - Dockerfile multi-stage + docker-compose (PostgreSQL 17 + API)
    - Scalar remplace Swagger (compatible .NET 10) → `/scalar/v1`
    - DevelopmentEmailService (log magic link en console)
    - Ports alignés 5000/5001
    - User Secrets configurés (DB, JWT, SMTP)
    - .dockerignore

**Total commits session du 9 Fév (suite)** : 3 commits (2 fixes + 1 Docker/Scalar)

#### Feature 4 : Navigation GPS — Backend
35. **8df5d14** - `feat(core): ajoute entités Trip, TripWaypoint et enums GPS`
    - 3 Enums : TripStatus (Recording/MonitorOnly/Finished), RouteProfile (Fast/Scenic), WaypointType (Start/Stopover/Destination)
    - Entité Trip : factory Create(), Finish(), AddWaypoint(), RemoveWaypoint() — validation domaine
    - Entité TripWaypoint : factory Create() avec validation lat [-90,90], lon [-180,180] — UpdateOrder()
    - Modification Convoy : ajout collection `ICollection<Trip> Trips`
    - Interface ITripRepository (GetById, GetActiveByConvoyId, GetByConvoyId, Add, Update)
36. **e820e89** - `feat(shared): ajoute DTOs pour les Voyages GPS`
    - StartTripRequest, CreateWaypointRequest, AddWaypointRequest (records, int pour enums)
    - TripDto, TripDetailsDto, TripWaypointDto (classes, int pour enums)
    - Respect architecture : Shared ne référence PAS Core → enums en `int`, Application fait le cast
37. **19dbebf** - `feat(application): ajoute commands, queries et validators pour les Voyages`
    - 4 Commands + Handlers : StartTrip, EndTrip, AddWaypoint, RemoveWaypoint
    - 3 Queries + Handlers : GetTripById, GetActiveTripByConvoyId, GetConvoyTrips
    - 2 Validators FluentValidation : StartTripValidator, AddWaypointValidator
38. **6acc7a1** - `feat(infrastructure): ajoute repository, configurations EF Core et migration Trip`
    - TripConfiguration : Table "Trips", FK Convoy (Cascade), enum→int, index (ConvoyId, Status)
    - TripWaypointConfiguration : Table "TripWaypoints", FK Trip (Cascade), FK User (Restrict), lat/lon precision(10,7)
    - TripRepository avec Include chains (Convoy.Members, Waypoints.AddedByUser)
    - DI registration ITripRepository → TripRepository
    - Migration AddTripFeature
39. **b3ca00b** - `feat(api): ajoute TripsController et TripHub SignalR`
    - TripsController : 7 endpoints REST nested sous `/api/convoys/{convoyId:guid}/trips`
      - POST `/` (StartTrip), GET `/active`, GET `/{tripId}`, GET `/` (historique)
      - POST `/{tripId}/end`, POST `/{tripId}/waypoints`, DELETE `/{tripId}/waypoints/{waypointId}`
    - TripHub SignalR : JoinTrip, LeaveTrip, SendLocationUpdate, SendRouteUpdate
    - Positions GPS éphémères (relayées via SignalR, PAS stockées en DB)
    - Program.cs : AddSignalR(), JWT query string `access_token` pour SignalR, MapHub("/hubs/trip")
40. **935a623** - `test: ajoute tests unitaires pour les Voyages GPS (42 tests)`
    - TripTests (14) : Create valid/Recording/MonitorOnly/Finished→exception/EmptyConvoyId, StartTime/EndTime, Finish, AddWaypoint, RemoveWaypoint
    - TripWaypointTests (13) : Create valid/empty fields/lat-lon boundaries, UpdateOrder
    - StartTripCommandHandlerTests (5) : valid/with waypoints/convoy not found/not leader/active trip exists
    - EndTripCommandHandlerTests (4) : valid/trip not found/not leader/already finished
    - AddWaypointCommandHandlerTests (4) : valid/trip not found/not member/finished trip
    - GetTripByIdQueryHandlerTests (2) : found/not found

41. **9270307** - `docs: met à jour PROGRESS.md avec Feature 4 Backend complète`

**Validation effectuée** :
- ✅ Build API : 0 erreurs
- ✅ Tous les tests passent (230/230 - 100%)
  - Core.Tests : 151 (124 existants + 27 nouveaux)
  - Application.Tests : 79 (64 existants + 15 nouveaux)
- ✅ Migration EF Core générée sans erreur
- ✅ Architecture respectée (Shared sans ref Core, enums castés dans Application)

**Total commits Feature 4 Backend** : 7 commits (core + shared + application + infrastructure + api + tests + docs)

---

### Session du 12 Février 2026

#### Feature 5 : Système de Vote — Backend
42. **3a3d1a1** - `feat(core): ajoute entités StopProposal, Vote et enums pour le système de vote`
    - 2 Enums : StopType (Fuel/Break/Food/Photo), ProposalStatus (Pending/Accepted/Rejected)
    - Entité StopProposal : factory Create(), CastVote(), Resolve(règle du silence), AllMembersVoted(), SetCreatedWaypoint()
    - Entité Vote : factory Create() avec validation
    - Interface IStopProposalRepository (GetById, GetPendingByTripId, GetExpiredPending, GetByTripId, Add, Update)
43. **e3ab1d1** - `feat(shared): ajoute DTOs pour le système de vote`
    - ProposeStopRequest, CastVoteRequest (records, int pour enums)
    - StopProposalDto, VoteDto (classes, YesCount/NoCount calculés)
44. **65bd577** - `feat(application): ajoute commands, queries et validators pour le système de vote`
    - 2 Commands + Handlers : ProposeStop (auto-vote YES), CastVote (résolution anticipée + waypoint auto)
    - 2 Queries + Handlers : GetActiveProposal, GetProposalHistory
    - 2 Validators FluentValidation
    - Interface ITripNotificationService (abstraction SignalR dans Application)
45. **0e39d89** - `feat(infrastructure): ajoute repository, configurations EF Core et background service pour le vote`
    - StopProposalRepository avec Include chains
    - StopProposalConfiguration : Table "StopProposals", FK Trip (Cascade), FK User (Restrict), index (TripId,Status) et (Status,ExpiresAt)
    - VoteConfiguration : Table "Votes", FK StopProposal (Cascade), FK User (Restrict), index unique (StopProposalId,UserId)
    - ProposalResolutionService : BackgroundService poll 5s, résolution propositions expirées, création waypoints auto
    - DI registration + Migration AddVotingSystem
46. **d8a07f4** - `feat(api): ajoute VotingController et TripNotificationService`
    - VotingController : 4 endpoints nested sous `/api/convoys/{convoyId}/trips/{tripId}/proposals`
    - TripNotificationService : implémentation ITripNotificationService via IHubContext<TripHub>
    - Events SignalR : StopProposed, VoteUpdate, ProposalResolved
    - Program.cs : registration ITripNotificationService
47. **2dbb5f9** - `test: ajoute tests unitaires pour le système de vote (60 tests)`
    - StopProposalTests (28) : Create, CastVote, Resolve règle du silence, AllMembersVoted, SetCreatedWaypoint
    - VoteTests (6) : Create, guards
    - ProposeStopCommandHandlerTests (7) : success, auto-vote, notification, trip not found, finished trip, not member, existing proposal
    - CastVoteCommandHandlerTests (8) : yes/no vote, notification, early resolution, waypoint creation, not found, not member, already voted
    - GetActiveProposalQueryHandlerTests (2) : found, not found
    - GetProposalHistoryQueryHandlerTests (2) : list, empty

**Validation effectuée** :
- ✅ Build API : 0 erreurs
- ✅ Tous les tests passent (290/290 - 100%)
  - Core.Tests : 192 (151 existants + 41 nouveaux)
  - Application.Tests : 98 (79 existants + 19 nouveaux)
- ✅ Migration EF Core générée sans erreur
- ✅ Architecture respectée (Shared sans ref Core, enums castés dans Application)
- ✅ Règle du silence : majorité absolue NON requise pour rejeter, sinon acceptée par défaut

**Total commits Feature 5 Backend** : 6 commits (core + shared + application + infrastructure + api + tests)

#### Documentation & Correctifs Mobile
48. **ca772c6** - `docs: met à jour ARCHITECTURE.md et INSTRUCTIONS.md avec état actuel du projet`
    - ARCHITECTURE.md v1.0→v1.2 : entités Voting, SignalR events, DI lifetimes, business rules
    - INSTRUCTIONS.md v1.0→v1.1 : cross-layer services pattern, checklists mis à jour
49. **18b9a7c** - `fix(mobile): corrige les fondations de l'application mobile`
    - Restructuration AppShell (login/main séparés)
    - AuthorizationMessageHandler (JWT Bearer automatique)
    - AddVehiclePage.xaml.cs créé, icônes SVG, typos corrigés, MainPage supprimé

**Total commits session du 12 Fév (Feature 5 + docs + fix)** : 9 commits (6 Feature 5 + 2 docs + 1 fix mobile)

#### Feature 6 : Chat — Backend
50. `feat(core): ajoute entite Message et IMessageRepository pour le chat`
    - Entité Message : factory Create(), validation contenu (non vide, max 500 chars)
    - Interface IMessageRepository (GetByConvoyIdAsync pagination curseur, AddAsync)
51. `feat(shared): ajoute DTOs pour le chat`
    - SendMessageRequest (record), MessageDto (class avec SenderUsername, SenderAvatarUrl)
52. `feat(application): ajoute commands, queries et validators pour le chat`
    - SendMessageCommand + Handler (crée message, persiste, notifie SignalR)
    - GetConvoyMessagesQuery + Handler (pagination curseur before + pageSize)
    - SendMessageValidator FluentValidation
    - Interface IConvoyNotificationService
53. `feat(infrastructure): ajoute repository et configuration EF Core pour le chat`
    - MessageRepository avec Include Sender
    - MessageConfiguration (FK Convoy Cascade, FK User Restrict, index ConvoyId+SentAt DESC)
    - DI registration
54. `feat(api): ajoute MessagesController, ConvoyHub et ConvoyNotificationService`
    - MessagesController : POST + GET sous `/api/convoys/{convoyId}/messages`
    - ConvoyHub SignalR `/hubs/convoy` (JoinConvoy, LeaveConvoy)
    - ConvoyNotificationService via IHubContext<ConvoyHub>
    - Program.cs : registration + MapHub
55. `test: ajoute tests unitaires pour le chat`
    - MessageTests (7) : Create valid, 500 chars OK, empty convoyId, empty senderId, empty/whitespace content, >500 chars
    - SendMessageCommandHandlerTests (5) : success + notification + DTO mapping + convoy not found + not member
    - GetConvoyMessagesQueryHandlerTests (4) : returns messages, empty list, convoy not found, not member

**Validation effectuée** :
- ✅ Build API : 0 erreurs
- ✅ Tous les tests passent (306/306 - 100%)
  - Core.Tests : 199 (192 existants + 7 nouveaux)
  - Application.Tests : 107 (98 existants + 9 nouveaux)
- ✅ Architecture respectée (nouveau ConvoyHub séparé du TripHub, IConvoyNotificationService)

**Total commits Feature 6 Backend** : 6 commits (core + shared + application + infrastructure + api + tests)

#### Correctifs Infrastructure & Validation Docker
56. **494a93f** - `fix(infra): corrige port PostgreSQL Docker (5433) pour eviter conflit avec instance locale`
57. **7184721** - `feat(infrastructure): ajoute migration EF Core AddChatFeature et supprime anciennes migrations`
58. **94b3301** - `fix(infrastructure): corrige DesignTimeDbContextFactory pour PostgreSQL Docker`
    - Connection string : port 5433, DB synctrip, password synctrip_dev_2026
    - `dotnet ef migrations list` fonctionne sans `--connection`

**Validation Docker effectuée** :
- ✅ `docker compose up --build -d` → 2 conteneurs UP (synctrip-db + synctrip-api)
- ✅ Migrations appliquées automatiquement ("Database migrations applied successfully!")
- ✅ Scalar UI accessible (`http://localhost:5000/scalar/v1` → 200 OK)
- ✅ Flux auth complet testé : magic-link → verify → register → JWT obtenu
- ✅ Endpoints authentifiés : GET /api/Brands, GET /api/Users/me → OK
- ✅ ProposalResolutionService démarré dans les logs

**Total commits correctifs infra** : 3 commits

### Session du 13 Février 2026

#### Feature 4 : Navigation GPS — Mobile
59. **4888742** - `feat(mobile): ajoute DeleteAsync a IApiService`
    - IApiService + ApiService : nouvelle méthode DeleteAsync(endpoint)
60. **88291c0** - `feat(mobile): ajoute ITripService pour les voyages GPS`
    - ITripService + TripService : 5 méthodes (StartTrip, GetActiveTrip, GetTripById, GetConvoyTrips, EndTrip)
61. **dc89f1a** - `feat(mobile): ajoute ISignalRService pour le temps reel GPS`
    - ISignalRService + SignalRService : HubConnection TripHub, JWT via AccessTokenProvider
    - Events : LocationReceived, méthodes ConnectAsync/SendLocationAsync/DisconnectAsync
62. **1d31e54** - `feat(mobile): ajoute ConvoyDetailPage avec gestion des voyages`
    - ConvoyDetailViewModel : LoadDetails, StartTrip, OpenCockpit, EndTrip
    - ConvoyDetailPage.xaml : affichage membres, voyage actif, boutons leader
    - TripStatusConverter, ConvoyRoleConverter (int → texte français)
63. **34b6ddd** - `feat(mobile): ajoute navigation ConvoyLobby vers ConvoyDetail`
    - SelectConvoyCommand + TapGestureRecognizer sur les cartes convoi
64. **3f2f78f** - `feat(mobile): ajoute CockpitPage avec carte Mapsui et positions GPS`
    - CockpitViewModel : géolocalisation 5s, SignalR temps réel, MemberPositions
    - CockpitPage : MapControl Mapsui, MyLocationLayer, WritableLayer membres, overlay info
65. **6a0bd45** - `feat(mobile): enregistre services et routes Feature 4 GPS`
    - MauiProgram.cs : UseSkiaSharp, TripService, SignalRService, ConvoyDetailVM, CockpitVM
    - AppShell.xaml.cs : routes convoydetail, cockpit

**Validation effectuée** :
- ✅ Build Mobile Windows : 0 erreurs (net10.0-windows10.0.19041.0)
- ✅ Navigation : ConvoyLobby → ConvoyDetail → CockpitPage
- ✅ Carte Mapsui : OpenStreetMap tiles, MyLocationLayer, WritableLayer
- ✅ SignalR : ConnectAsync/DisconnectAsync avec JWT, ReceiveLocationUpdate

**Total commits Feature 4 Mobile** : 7 commits

#### Migration MAUI → AvaloniaUI
**Statut** : TERMINÉ
**Date** : 13 Février 2026

**Motivation** :
- AvaloniaUI couvre **toutes les plateformes** : Windows, macOS, Linux, iOS, Android, WebAssembly
- MAUI ne supporte pas Linux ni WebAssembly
- `Mapsui.Avalonia 5.0.0` compatible — migration directe depuis Mapsui.Maui
- `CommunityToolkit.Mvvm` est framework-agnostic — tous les ViewModels réutilisables
- Backend (Core, Shared, Application, Infrastructure, API) : **aucun changement**
- Tests (306/306) : **aucun changement**

**11 étapes réalisées** :
1. ✅ Scaffold projet Avalonia (SyncTrip.App + SyncTrip.App.Desktop + stubs Android/iOS/Browser)
2. ✅ Interfaces d'abstraction plateforme (INavigationService, IDialogService, ISecureStorageService, ILocationService)
3. ✅ Port 8 services métier (namespace + ISecureStorageService au lieu de MAUI SecureStorage)
4. ✅ Port 7 convertisseurs (Avalonia.Data.Converters, Brushes au lieu de Colors)
5. ✅ Port 10 ViewModels (NavigationService, DialogService, Initialize() au lieu de QueryProperty)
6. ✅ ViewLocator, NavigationService stack-based, DialogService, DesktopSecureStorageService, thème Colors.axaml
7. ✅ Vues Authentication (MagicLinkView, RegistrationView)
8. ✅ Vues Profile et Garage (ProfileView, GarageView, AddVehicleView)
9. ✅ Vues Convoy (ConvoyLobbyView, CreateConvoyView, JoinConvoyView, ConvoyDetailView)
10. ✅ CockpitView avec Mapsui.Avalonia 5.0.0 (MapControl, WritableLayer, OpenStreetMap)
11. ✅ MainView TabControl (Profil, Garage, Convois) + finalisation DI + build 0 erreurs + tests 306/306

**Changements techniques notables** :
- Avalonia 11.3.1 (upgrade depuis 11.3.0 pour compatibilité Mapsui 5.0.0)
- `SyncTrip.Mobile` supprimé → `SyncTrip.App` (UI partagée) + `SyncTrip.App.Desktop` (head)
- Navigation : INavigationService custom + ViewLocator convention (XxxViewModel → XxxView)
- Mapsui 5.0 API : `WritableLayer()` sans argument, `SphericalMercator.FromLonLat` retourne tuple, `Navigator.CenterOnAndZoomTo`
- ProfileViewModel : HasLicenseB/A/C/D pour binding CheckBox pur MVVM (plus de code-behind)
- SwipeView MAUI → boutons inline + ItemsControl Avalonia

**Validation** :
- ✅ `dotnet build "src\SyncTrip.App.Desktop\SyncTrip.App.Desktop.csproj"` — 0 erreurs
- ✅ `dotnet test` — 306/306 (199 Core + 107 Application)

#### Correctifs Services HTTP + SMTP + Tests Fonctionnels
66. `fix(app): corrige services HTTP, verification token et SMTP pour tests fonctionnels`
    - ApiService : ajout PutAsync, ReadResponseAsync<T> (extraction Guid depuis JSON wrappé)
    - UserService : URL `api/users/profile` → `api/users/me`, POST → PUT
    - VehicleService : POST → PUT (update), POST → DELETE (delete)
    - AuthenticationService : suppression check `response?.Success`
    - MagicLinkViewModel : ajout étape 2 vérification token (copier-coller)
    - CompleteRegistrationCommandHandler : retourne JWT pour utilisateur existant (idempotent)
    - Infrastructure : force EmailService (vrai SMTP Gmail) même en dev
    - App : BaseAddress corrigé `http://localhost:5000`

#### Ameliorations Desktop : Destination + Geolocalisation
67. `feat(app): ajoute geolocalisation IP pour DesktopLocationService`
    - DesktopLocationService : ip-api.com/json (lat/lon), cache session, fallback Paris
68. `feat(app): ajoute saisie destination lors du demarrage d'un voyage`
    - ConvoyDetailViewModel : champs DestinationName/Latitude/Longitude, validation, toggle form
    - ConvoyDetailView : formulaire destination (nom + lat/lon), boutons Annuler/Demarrer
    - StartTrip : envoie waypoint Type=3 (Destination) dans StartTripRequest.Waypoints
69. `feat(app): affiche destination et positions sur la carte Mapsui`
    - CockpitView : WritableLayer waypoints (destination=rouge, stopover=orange, start=vert) avec LabelStyle
    - CockpitView : label "Moi" sur position utilisateur, bounding box centrage auto
    - CockpitViewModel : DestinationName, WaypointsLoaded event
    - CockpitView.axaml : overlay destination dans panneau info

**Validation** :
- ✅ Build Desktop : 0 erreurs
- ✅ Tests : 306/306 (199 Core + 107 Application)
- ✅ Backend : 0 changement nécessaire (waypoints déjà supportés)

#### Backend Geocoding + Routing (Navigation)
70. `feat(core): ajoute interfaces IGeocodingService et IRoutingService + route Trip`
    - IGeocodingService (SearchAsync → GeocodingResult), IRoutingService (CalculateRouteAsync → RouteResult)
    - Trip : RouteGeometry, RouteDistanceMeters, RouteDurationSeconds + UpdateRoute/ClearRoute
71. `feat(shared): ajoute DTOs Navigation pour geocodage et itineraire`
    - AddressResultDto, CalculateRouteRequest, RouteResultDto, RouteStepDto
    - TripDetailsDto enrichi avec champs route
72. `feat(application): ajoute handlers Navigation (geocodage, routing, calcul voyage)`
    - SearchAddressQuery, CalculateRouteQuery, CalculateTripRouteCommand + handlers
    - 3 validators FluentValidation + mapping route dans query handlers Trip
73. `feat(infrastructure): ajoute services Nominatim et OSRM + migration route`
    - NominatimGeocodingService (geocodage OpenStreetMap gratuit)
    - OsrmRoutingService (calcul itineraire GeoJSON, Scenic=exclude motorway avec fallback)
    - HttpClient types dans DI + migration AddRouteGeometry
74. `feat(api): ajoute NavigationController avec geocodage et calcul itineraire`
    - GET /api/navigation/search (geocodage), POST /api/navigation/route (preview)
    - POST /api/navigation/trips/{tripId}/route (calcul + persistance)
75. `test: ajoute tests Navigation et Trip route (14 tests, 320/320)`
    - TripRouteTests (3), SearchAddressQueryHandler (3), CalculateRouteQueryHandler (3)
    - CalculateTripRouteCommandHandler (5) : valid, not found, non member, <2 waypoints, finished

**Validation effectuée** :
- ✅ Build API : 0 erreurs
- ✅ Tous les tests passent (320/320 - 100%)
  - Core.Tests : 202 (199 existants + 3 nouveaux)
  - Application.Tests : 118 (107 existants + 11 nouveaux)
- ✅ APIs externes gratuites sans clé (Nominatim + OSRM)
- ✅ Route persistée dans Trip (visible par tous les membres du convoi)

**Total commits Backend Geocoding/Routing** : 6 commits + 1 docs (ISSUES.md)

---

## Prochaines Actions

### Priorité Moyenne
1. Ajouter tests d'intégration API
2. Ajouter tests unitaires ViewModels (App)

### Priorité Basse
1. Améliorer UI/UX avec animations
2. Configurer CI/CD
3. Activer les platform heads Android/iOS/Browser (nécessite SDKs)
