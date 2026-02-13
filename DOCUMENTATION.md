# Master Documentation : SyncTrip
**Version** : 4.1 (Privacy, Voting Logic & DB Refactoring)
**Date** : 23 Novembre 2025
**Statut** : Validé pour Développement

---

## PARTIE A : SPÉCIFICATIONS FONCTIONNELLES & CONCEPT
Cette section décrit le comportement de l'application vu par l'utilisateur.

### 1. Vision Produit
SyncTrip est une application permettant de synchroniser un **Convoi de véhicules**. L'objectif est de :
- Maintenir la cohésion du groupe.
- Partager un itinéraire unique.
- Communiquer des besoins vitaux (essence, pause) **sans distraction**.

---

### 2. Gestion des Utilisateurs & Véhicules

#### 2.1 L'Identité (Onboarding)
- **Accès** : Passwordless (Magic Link).
- **Profil** :
  - Username (Pseudo) : **Obligatoire**.
  - Nom / Prénom : **Facultatifs**.
  - Date de naissance : **Obligatoire** (Règle : > 14 ans).
  - Permis : Déclaratifs (A, B, C...) via sélecteur.

#### 2.2 Le Garage (Véhicules)
- **Règle d'Or** : Un véhicule est requis pour rejoindre un convoi en tant que membre actif.
- **Données** :
  - Marque : Sélection via une liste officielle (ex: Yamaha, Renault) avec Logo.
  - Modèle : Champ libre.
  - Détails : Couleur, Année (**Optionnels**).
  - **Visuel** : Ces infos permettent d'identifier les amis sur la route (*"Je suis la Clio Rouge"*).

---

### 3. Le Convoi (Lobby & Gestion)

#### 3.1 Création et Accès
- **Code Convoi** : Généré automatiquement (ex: K9P-2XL).
- **Modes** : Ouvert (Accès direct) ou Privé (Validation par Leader).

#### 3.2 Rôles et Permissions
- **Pas de distinction** entre "Conducteur" et "Passager". Tout participant est un **Membre**.
- **Leader (Admin)** :
  - Possède tous les droits d'un Membre.
  - **Droits exclusifs** :
    - Gestion des membres (Kick/Ban/Validation).
    - Définition de la Destination finale.
    - Passation de pouvoir (Nommer un nouveau leader).
    - Dissolution du convoi.
- **Membre** :
  - Droit de voir la carte.
  - Droit de proposer un arrêt / Voter.
  - Droit de participer au chat.

---

### 4. Le Voyage (Navigation & Road Trip)

#### 4.1 Distinction Convoi vs Voyage
- **Convoi** : Le groupe social.
- **Voyage** : L'activité GPS.

#### 4.2 Modes de Voyage & Confidentialité
- **Confidentialité (Privacy First)** :
  - La position GPS n'est partagée que si l'application est **active au premier plan** (ou via un service foreground notifié).
  - Si un utilisateur ferme l'application ou perd le réseau, son avatar se fige (devient gris) ou disparaît.
  - **Aucun tracking silencieux** en arrière-plan si l'utilisateur n'est pas "dans" le voyage.
- **Modes** :
  - **Balade (Free Drive)** : Bouton REC pour enregistrer le tracé. Bouton STOP pour archiver.
  - **Road Trip (Guidé)** : Une destination est définie.

#### 4.3 Gestion des Arrêts (Vote Implicite)
- **Itinéraire vivant** :
  - **Intention** : Un membre clique sur "Essence".
  - **Règle** : Cette demande compte automatiquement comme un **VOTE OUI** de sa part.
  - **Broadcast** : Notification à tout le groupe avec compte à rebours (ex: 30 secondes).
  - **Système de Vote** :
    - Les autres membres peuvent voter 👍 ou 👎.
    - **Règle du Silence** : Si le temps est écoulé et que personne n'a voté (ou pas de majorité de NON), la proposition est **ACCEPTÉE** par défaut (*"Qui ne dit mot consent"*).
  - **Mise à jour Route** : Le système ajoute la station la plus proche comme étape (**Waypoint**) dans `TripWaypoints`.

---

### 5. L'Interface de Conduite ("Le Cockpit")
- **La Carte** : Tracé + Avatars.
- **Le Roster (Tuiles)** : Bandeau haut défilant. Tuiles avec Photo + Pseudo/Véhicule + Distance.
- **Le Stream (Chat)** : Lecture seule, transparent, cliquable pour ouvrir le chat complet.
- **Le Deck (Actions)** : En bas à droite. 4 boutons (Pause, Essence, Miam, Photo).
- **Le Bouton SOS** : Séparé visuellement (isolé) pour éviter les erreurs.

---

## PARTIE B : SPÉCIFICATIONS TECHNIQUES
Cette section décrit l'implémentation stricte.

**Stack** : .NET 10 AvaloniaUI / ASP.NET Core / PostgreSQL / Redis / SignalR / Mapsui.

### 1. Module Authentification (Blind Send & Proof)
#### 1.1 Flux Magic Link
- **Envoi** (POST `/auth/magic-link`) : Aveugle.
- **Vérification** (POST `/auth/verify`) : Vérifie token. Si email inconnu → JWT RegistrationScope.
- **Inscription** (POST `/profile`) : Crée l'user.

---

### 2. Modèle de Données (PostgreSQL - Normalisé)

#### Table `Users`
| Champ | Type | Contrainte |
|-------|------|------------|
| Id | UUID, PK |  |
| Email | Unique, Not Null |  |
| Username | Not Null |  |
| FirstName, LastName | Nullable |  |
| BirthDate | Date, Not Null |  |
| AvatarUrl | Nullable |  |
| CreatedAt, UpdatedAt | Timestamp |  |
| IsActive | Bool, Default True |  |
| DeactivationDate | Timestamp, Nullable |  |

#### Table `UserLicenses` (Liaison User-Permis)
- **Pas de table de référence** `Ref_Licenses`. Utilisation d'un **Enum C#** mappé.
- **Champs** :
  - UserId (FK Users)
  - LicenseType (Int) → Mappe l'Enum `LicenseType { AM=1, A1=2, B=3... }`
  - **Constraint** : PK Composite (UserId, LicenseType).

#### Table `Brands` (Marques Officielles)
| Champ | Type |
|-------|------|
| Id | Int, PK |
| Name | String: "Yamaha", "Renault" |
| LogoUrl | String |

#### Table `Vehicles`
| Champ | Type | Contrainte |
|-------|------|------------|
| Id | UUID, PK |  |
| UserId | FK Users |  |
| BrandId | FK Brands, Not Null |  |
| Model | String, Not Null |  |
| Type | Enum: Car, Moto, Truck |  |
| Color | String, Nullable |  |
| Year | Int, Nullable |  |

#### Table `Convoys`
| Champ | Type |
|-------|------|
| Id | UUID, PK |
| JoinCode | String 6 chars, Unique Index |
| LeaderUserId | FK Users |
| IsPrivate | Bool |
| CreatedAt | Timestamp |

#### Table `Trips` (Actifs et Historique)
| Champ | Type |
|-------|------|
| Id | UUID, PK |
| ConvoyId | FK Convoys |
| Status | Enum: Recording, MonitorOnly, Finished |
| StartTime, EndTime | Timestamp |
| RouteProfile | Enum: Fast, Scenic |

#### Table `TripWaypoints` (Itinéraire Dynamique)
| Champ | Type |
|-------|------|
| Id | UUID, PK |
| TripId | FK Trips |
| OrderIndex | Int |
| Latitude, Longitude |  |
| Name |  |
| Type | Enum: Start, Stopover, Destination |
| AddedByUserId | FK Users |
| **Note** : C'est cette table qui reçoit les INSERT quand un vote "Arrêt Essence" est validé. |

---

### 3. Architecture Temps Réel (SignalR)
#### 3.1 Hub Events
- `UserJoined`, `UserLeft`
- `LocationUpdated(UserId, Lat, Lon)` → Seulement si app active.
- `RouteUpdated(GeoJson)`
- `StopProposed(StopDetails)`
- `VoteUpdate(StopId, YesCount, NoCount)`

---

### 4. Qualité & Développement
- **Direct-to-Prod** : Pas de placeholders fonctionnels.
- **Git Flow** : `main`, `develop`, `feat/xxx`.
- **Clean Arch** : Respect strict des couches.
