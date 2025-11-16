# SyncTrip API - Guide de Démarrage Rapide

## 🎯 Backend API Complètement Implémenté !

L'API backend SyncTrip est maintenant **100% fonctionnelle** avec :

✅ Architecture N-tiers complète (Core → Infrastructure → Application → API)
✅ Repository Pattern générique + UnitOfWork
✅ 8 Entités du domaine avec BaseEntity
✅ 5 Services métier avec toute la logique
✅ 4 Controllers REST (Auth, Convoys, Trips, Messages)
✅ 2 Hubs SignalR temps réel (Location, Chat)
✅ AutoMapper configuré
✅ FluentValidation pour toutes les requêtes
✅ JWT Authentication + Magic Links
✅ SignalR pour GPS et chat en temps réel
✅ Logging avec Serilog
✅ Swagger intégré

---

## 🚀 Démarrage de l'API

### 1. Démarrer PostgreSQL et Redis

```bash
# Démarrer Docker Desktop puis :
docker-compose up -d
```

Cela lance :
- PostgreSQL 16 sur le port 5432
- Redis 7 sur le port 6379

### 2. Appliquer les migrations

```bash
cd SyncTrip.Api
dotnet ef database update
```

### 3. Lancer l'API

```bash
dotnet run
```

L'API démarre sur :
- HTTPS : `https://localhost:5001`
- HTTP : `http://localhost:5000`
- **Swagger UI** : `https://localhost:5001` (page d'accueil)

---

## 📡 Endpoints Disponibles

### 🔐 **Authentification** (`/api/auth`)

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| POST | `/api/auth/register` | Inscription d'un nouvel utilisateur |
| POST | `/api/auth/send-magic-link` | Demande d'envoi d'un magic link |
| POST | `/api/auth/verify-magic-link` | Vérification du magic link et obtention du JWT |

**Exemple d'inscription :**
```json
POST /api/auth/register
{
  "email": "user@example.com",
  "displayName": "John Doe",
  "phoneNumber": "+33612345678"
}
```

**Exemple de magic link :**
```json
POST /api/auth/send-magic-link
{
  "email": "user@example.com"
}
```

Le token apparaîtra dans les logs (en attendant l'intégration d'un service d'email).

### 🚗 **Convois** (`/api/convoys`)

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| POST | `/api/convoys` | Créer un nouveau convoi |
| POST | `/api/convoys/join` | Rejoindre un convoi avec son code |
| POST | `/api/convoys/{id}/leave` | Quitter un convoi |
| GET | `/api/convoys/{id}` | Récupérer un convoi par ID |
| GET | `/api/convoys/by-code/{code}` | Récupérer un convoi par code |
| GET | `/api/convoys/my-convoys` | Récupérer ses convois |
| PUT | `/api/convoys/{id}` | Mettre à jour un convoi |
| DELETE | `/api/convoys/{id}` | Supprimer un convoi |

**Exemple de création de convoi :**
```json
POST /api/convoys
Authorization: Bearer {token}
{
  "name": "Voyage en Pologne",
  "vehicleName": "Peugeot 308 grise"
}
```

Réponse :
```json
{
  "success": true,
  "message": "Convoi créé avec succès",
  "data": {
    "id": "...",
    "code": "A3bC5d",  ← Code unique de 6 caractères
    "name": "Voyage en Pologne",
    ...
  }
}
```

### 🗺️ **Trips** (`/api/trips`)

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| POST | `/api/trips` | Créer un nouveau trip |
| GET | `/api/trips/{id}` | Récupérer un trip |
| GET | `/api/trips/convoy/{convoyId}` | Récupérer tous les trips d'un convoi |
| GET | `/api/trips/convoy/{convoyId}/active` | Récupérer le trip actif |
| PATCH | `/api/trips/{id}/status` | Changer le statut d'un trip |
| POST | `/api/trips/{id}/waypoints` | Ajouter un waypoint |
| POST | `/api/trips/waypoints/{id}/reached` | Marquer un waypoint atteint |

**Exemple de création de trip :**
```json
POST /api/trips
Authorization: Bearer {token}
{
  "convoyId": "...",
  "destination": "Cracovie, Pologne",
  "destinationLatitude": 50.0647,
  "destinationLongitude": 19.9450,
  "routePreference": "Fastest",
  "plannedDepartureTime": "2025-12-01T08:00:00Z"
}
```

### 💬 **Messages** (`/api/convoys/{convoyId}/messages`)

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| POST | `/api/convoys/{convoyId}/messages` | Envoyer un message |
| GET | `/api/convoys/{convoyId}/messages?skip=0&take=50` | Récupérer les messages (paginés) |
| DELETE | `/api/convoys/{convoyId}/messages/{messageId}` | Supprimer un message |

---

## 🔌 Hubs SignalR (Temps Réel)

### 📍 **LocationHub** (`/hubs/location`)

**Méthodes disponibles :**
- `JoinTrip(tripId)` : Rejoindre la room d'un trip pour recevoir les positions
- `LeaveTrip(tripId)` : Quitter la room
- `UpdateLocation(tripId, { latitude, longitude, speed, ... })` : Mettre à jour sa position

**Events reçus :**
- `ReceiveAllLocations` : Positions initiales de tous les participants
- `LocationUpdated` : Mise à jour de position d'un participant

### 💬 **ChatHub** (`/hubs/chat`)

**Méthodes disponibles :**
- `JoinConvoyChat(convoyId)` : Rejoindre le chat d'un convoi
- `LeaveConvoyChat(convoyId)` : Quitter le chat
- `SendMessage(convoyId, { content })` : Envoyer un message

**Events reçus :**
- `ReceiveMessageHistory` : Historique des 50 derniers messages
- `ReceiveMessage` : Nouveau message reçu en temps réel

---

## 🧪 Tester l'API

### Avec Swagger UI

1. Ouvrir `https://localhost:5001`
2. Utiliser le bouton **"Authorize"** en haut à droite
3. Entrer le token JWT : `Bearer {votre_token}`
4. Tester tous les endpoints directement dans l'interface

### Avec cURL / Postman / Thunder Client

Exemple complet de flux :

```bash
# 1. Inscription
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","displayName":"Test User"}'

# 2. Demander un magic link
curl -X POST https://localhost:5001/api/auth/send-magic-link \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com"}'

# 3. Récupérer le token dans les logs de l'API

# 4. Vérifier le magic link
curl -X POST https://localhost:5001/api/auth/verify-magic-link \
  -H "Content-Type: application/json" \
  -d '{"token":"LE_TOKEN_DES_LOGS"}'

# Réponse contient le JWT

# 5. Créer un convoi
curl -X POST https://localhost:5001/api/convoys \
  -H "Authorization: Bearer VOTRE_JWT" \
  -H "Content-Type: application/json" \
  -d '{"name":"Mon Convoi","vehicleName":"Tesla Model 3"}'
```

---

## 📊 Structure de l'API

```
SyncTrip.Api/
├── Core/
│   ├── Entities/          ← 8 entités + BaseEntity
│   ├── Enums/             ← 5 enums
│   └── Interfaces/        ← Interfaces des services & repositories
├── Infrastructure/
│   ├── Data/              ← DbContext, UnitOfWork, Migrations
│   ├── Repositories/      ← 8 repositories + Repository<T>
│   └── Services/          ← 5 services métier implémentés
├── Application/
│   ├── DTOs/              ← 20+ DTOs organisés par domaine
│   ├── Mappings/          ← AutoMapper profiles
│   └── Validators/        ← 7 FluentValidation validators
└── API/
    ├── Controllers/       ← 4 controllers REST
    └── Hubs/              ← 2 SignalR hubs
```

---

## ✅ Fonctionnalités Implémentées

### Authentification
- ✅ Inscription utilisateur
- ✅ Magic link passwordless (15min de validité)
- ✅ JWT tokens (1h d'expiration)
- ✅ 2FA prévu (structure en place)

### Convois
- ✅ Création de convoi avec code unique (6 caractères)
- ✅ Jointure par code
- ✅ Gestion des rôles (Owner/Admin/Member)
- ✅ Quitter un convoi
- ✅ Auto-archivage si vide
- ✅ Soft delete

### Trips
- ✅ Création de trip avec destination OBLIGATOIRE
- ✅ **UN SEUL trip actif par convoi** (règle métier appliquée)
- ✅ Gestion des statuts (Planned → InProgress → Completed)
- ✅ Waypoints ordonnés
- ✅ Marquage waypoint atteint

### Messages
- ✅ Chat groupe (pas de messages privés)
- ✅ Messages système automatiques
- ✅ Pagination
- ✅ Soft delete
- ✅ Temps réel via SignalR

### Positions GPS
- ✅ Mise à jour temps réel via SignalR
- ✅ Historique complet
- ✅ Dernière position de chaque participant
- ✅ Métadonnées (altitude, vitesse, cap, précision)

---

## 🔒 Sécurité

- ✅ JWT Authentication sur tous les endpoints (sauf auth)
- ✅ Validation des inputs avec FluentValidation
- ✅ Vérification des appartenances aux convois
- ✅ Soft delete (pas de suppression définitive)
- ✅ Query filters EF Core (exclure deleted par défaut)
- ✅ CORS configuré

---

## 📝 Logs

L'API utilise **Serilog** pour les logs.

Tous les événements importants sont loggés :
- Création de convois/trips
- Jointures/départs
- Envoi de messages
- Mises à jour de positions
- Erreurs

---

## 🎯 Prochaines Étapes

Pour une mise en production, il faudrait ajouter :

1. **Service d'email** pour les magic links (Sendgrid, Mailgun, etc.)
2. **Service SMS** pour la 2FA (Twilio, etc.)
3. **Rate limiting** (anti-spam)
4. **Métriques** (Application Insights, Prometheus)
5. **Tests unitaires** (xUnit)
6. **CI/CD** (GitHub Actions)
7. **Hébergement** (Azure App Service, AWS, etc.)

Mais le backend est **100% fonctionnel** et prêt à être utilisé ! 🚀

---

## 🐛 Debugging

**Problèmes courants :**

1. **"Unable to connect to database"**
   - Vérifier que Docker Desktop est démarré
   - `docker-compose ps` pour voir l'état des conteneurs
   - `docker-compose logs postgres` pour voir les logs

2. **"Token expired"**
   - Le JWT expire après 1h
   - Re-demander un magic link

3. **"Convoy already has an active trip"**
   - C'est normal ! Règle métier : 1 seul trip actif par convoi
   - Terminer le trip actuel avant d'en créer un nouveau

---

Bon développement ! 🎉
