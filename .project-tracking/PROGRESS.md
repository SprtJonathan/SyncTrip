# SyncTrip - Suivi de Progression

**Dernière mise à jour** : 23 Novembre 2025
**Statut Global** : En développement initial

---

## Vue d'ensemble

Le projet SyncTrip est développé en utilisant une **approche verticale (vertical slice)**.
Chaque feature est développée de bout en bout (Core → Application → Infrastructure → API → Mobile) avant de passer à la suivante.

---

## Progression par Feature

### ✅ TERMINÉ
_Aucune feature terminée pour le moment_

---

### 🚧 EN COURS

#### Feature 1 : Authentification (Magic Link)
**Statut** : Non démarré
**Progression** : 0%

**Composants** :
- [ ] Core : Entités User, MagicLinkToken
- [ ] Core : Interfaces IUserRepository, IAuthService, IEmailService
- [ ] Shared : DTOs Auth (MagicLinkRequest, VerifyTokenRequest, etc.)
- [ ] Application : Commands Auth (SendMagicLink, VerifyToken, CompleteRegistration)
- [ ] Application : Validators Auth
- [ ] Infrastructure : Repositories (UserRepository, MagicLinkTokenRepository)
- [ ] Infrastructure : Services (AuthService, EmailService)
- [ ] Infrastructure : Configuration EF Core + Migration initiale
- [ ] API : AuthController
- [ ] Mobile : MagicLinkPage + ViewModel
- [ ] Mobile : RegistrationPage + ViewModel
- [ ] Mobile : AuthenticationService
- [ ] Tests : Tests unitaires entités
- [ ] Tests : Tests handlers
- [ ] Tests : Tests API
- [ ] Vérification : Compilation sans erreur
- [ ] Vérification : Feature testée end-to-end

---

### 📋 À FAIRE

#### Feature 2 : Profil & Garage
**Statut** : Pas démarré
**Priorité** : Haute

**Composants** :
- [ ] Core : Entités Vehicle, Brand, UserLicense
- [ ] Shared : DTOs Users, Vehicles
- [ ] Application : Commands & Queries Users/Vehicles
- [ ] Infrastructure : Repositories & Seed Brands
- [ ] API : Controllers (Users, Vehicles, Brands)
- [ ] Mobile : Pages Profil + Garage
- [ ] Tests complets

---

#### Feature 3 : Convois
**Statut** : Pas démarré
**Priorité** : Haute

**Composants** :
- [ ] Core : Entités Convoy, ConvoyMember
- [ ] Core : Service ConvoyCodeGenerator
- [ ] Shared : DTOs Convoys
- [ ] Application : Commands Convoy
- [ ] Infrastructure : ConvoyRepository
- [ ] API : ConvoysController
- [ ] Mobile : Pages Convoy (Create/Join/Lobby)
- [ ] Tests complets

---

#### Feature 4 : Navigation GPS
**Statut** : Pas démarré
**Priorité** : Haute

**Composants** :
- [ ] Core : Entités Trip, TripWaypoint, LocationHistory
- [ ] Shared : DTOs Trips, Location
- [ ] Application : Commands Trip
- [ ] Infrastructure : TripRepository
- [ ] API : TripsController + TripHub (SignalR)
- [ ] Mobile : CockpitPage + MapControl (Mapsui)
- [ ] Mobile : LocationService (foreground only)
- [ ] Tests complets

---

#### Feature 5 : Système de Vote
**Statut** : Pas démarré
**Priorité** : Moyenne

**Composants** :
- [ ] Core : Entités StopProposal, Vote
- [ ] Core : Logique "règle du silence"
- [ ] Shared : DTOs Voting
- [ ] Application : Commands Voting
- [ ] Infrastructure : VotingRepository + Background Job
- [ ] API : VotingController + SignalR events
- [ ] Mobile : VotingModal + DeckControl
- [ ] Tests complets (surtout règle du silence)

---

#### Feature 6 : Chat
**Statut** : Pas démarré
**Priorité** : Basse

**Composants** :
- [ ] Core : Entité Message
- [ ] Shared : DTOs Messages
- [ ] Application : Commands Messages
- [ ] Infrastructure : MessageRepository
- [ ] API : ConvoyHub (SignalR)
- [ ] Mobile : ChatPage + ChatStreamControl
- [ ] Tests complets

---

## Métriques

**Features Terminées** : 0 / 6
**Progression Globale** : 0%
**Dernière compilation** : N/A
**Tests Passing** : 0 / 0

---

## Historique des Commits

_Les commits seront listés ici au fur et à mesure_

### Session du 23 Novembre 2025
- Aucun commit pour le moment

---

## Prochaines Actions

1. Créer les fichiers de suivi (.project-tracking)
2. Démarrer Feature 1 : Authentification
3. Commits fréquents après chaque fichier/groupe compilable
4. Vérifier compilation avant chaque commit
