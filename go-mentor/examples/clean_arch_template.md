# Эталонный шаблон Clean Architecture на Go

Этот шаблон демонстрирует правильное разделение ответственности без излишней сложности («overengineering»).

```text
my-service/
├── cmd/
│   └── api/
│       └── main.go              # Точка входа: чтение конфига, DI, запуск сервера
├── internal/
│   ├── domain/                  # Сущности и ошибки бизнес-логики (нет внешних зависимостей)
│   │   ├── user.go
│   │   └── errors.go
│   ├── service/                 # Use cases (бизнес-сценарии)
│   │   ├── user_service.go
│   │   └── user_service_test.go
│   ├── repository/              # Адаптер к БД (PostgreSQL, SQLite)
│   │   └── pg_user_repo.go
│   └── delivery/
│       └── http/                # Входной транспорт: обработчики HTTP (Chi/Gin)
│           ├── handler.go
│           └── response.go
├── pkg/                         # Вспомогательные пакеты общего назначения
├── go.mod
└── go.sum
```

---

## 1. Слой Domain (`internal/domain/user.go`)
Содержит чистые структуры данных и доменные ошибки. Не импортирует `net/http`, драйверы БД и сторонние библиотеки.

```go
package domain

import (
	"errors"
	"time"
)

var (
	ErrUserNotFound = errors.New("user not found")
	ErrInvalidEmail = errors.New("invalid email address")
)

type User struct {
	ID        int64     `json:"id"`
	Email     string    `json:"email"`
	Name      string    `json:"name"`
	CreatedAt time.Time `json:"created_at"`
}
```

---

## 2. Слой Service / Use Case (`internal/service/user_service.go`)
Содержит бизнес-логику. **Здесь объявляется интерфейс репозитория** (принцип инверсии зависимостей).

```go
package service

import (
	"context"
	"fmt"
	"strings"

	"my-service/internal/domain"
)

// UserRepository объявляется потребителем (service), а не поставщиком (repository)
type UserRepository interface {
	GetByID(ctx context.Context, id int64) (*domain.User, error)
	Create(ctx context.Context, user *domain.User) error
}

type UserService struct {
	repo UserRepository
}

func NewUserService(repo UserRepository) *UserService {
	return &UserService{repo: repo}
}

func (s *UserService) Register(ctx context.Context, email, name string) (*domain.User, error) {
	if !strings.Contains(email, "@") {
		return nil, domain.ErrInvalidEmail
	}

	user := &domain.User{
		Email: email,
		Name:  name,
	}

	if err := s.repo.Create(ctx, user); err != nil {
		return nil, fmt.Errorf("creating user in repository: %w", err)
	}

	return user, nil
}
```

---

## 3. Слой Repository (`internal/repository/pg_user_repo.go`)
Реализует интерфейс `UserRepository` через работу с реальной БД (например, через `jackc/pgx`).

```go
package repository

import (
	"context"
	"errors"
	"fmt"

	"github.com/jackc/pgx/v5"
	"github.com/jackc/pgx/v5/pgxpool"
	"my-service/internal/domain"
)

type PgUserRepository struct {
	pool *pgxpool.Pool
}

func NewPgUserRepository(pool *pgxpool.Pool) *PgUserRepository {
	return &PgUserRepository{pool: pool}
}

func (r *PgUserRepository) GetByID(ctx context.Context, id int64) (*domain.User, error) {
	query := `SELECT id, email, name, created_at FROM users WHERE id = $1`
	var u domain.User
	err := r.pool.QueryRow(ctx, query, id).Scan(&u.ID, &u.Email, &u.Name, &u.CreatedAt)
	if err != nil {
		if errors.Is(err, pgx.ErrNoRows) {
			return nil, domain.ErrUserNotFound
		}
		return nil, fmt.Errorf("querying user: %w", err)
	}
	return &u, nil
}

func (r *PgUserRepository) Create(ctx context.Context, u *domain.User) error {
	query := `INSERT INTO users (email, name) VALUES ($1, $2) RETURNING id, created_at`
	err := r.pool.QueryRow(ctx, query, u.Email, u.Name).Scan(&u.ID, &u.CreatedAt)
	if err != nil {
		return fmt.Errorf("inserting user: %w", err)
	}
	return nil
}
```

---

## 4. Слой Delivery / HTTP (`internal/delivery/http/handler.go`)
Принимает HTTP-запрос, декодирует JSON, вызывает сервис, конвертирует доменные ошибки в HTTP статус-коды.

```go
package http

import (
	"encoding/json"
	"errors"
	"net/http"

	"my-service/internal/domain"
	"my-service/internal/service"
)

type UserHandler struct {
	service *service.UserService
}

func NewUserHandler(s *service.UserService) *UserHandler {
	return &UserHandler{service: s}
}

type registerRequest struct {
	Email string `json:"email"`
	Name  string `json:"name"`
}

func (h *UserHandler) Register(w http.ResponseWriter, r *http.Request) {
	var req registerRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "bad request", http.StatusBadRequest)
		return
	}

	user, err := h.service.Register(r.Context(), req.Email, req.Name)
	if err != nil {
		if errors.Is(err, domain.ErrInvalidEmail) {
			http.Error(w, err.Error(), http.StatusBadRequest)
			return
		}
		http.Error(w, "internal server error", http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusCreated)
	_ = json.NewEncoder(w).Encode(user)
}
```

---

## 5. Точка сборки (DI) (`cmd/api/main.go`)
Только `main.go` знает обо всех слоях, создает зависимости снизу вверх и связывает их воедино.

```go
package main

import (
	"context"
	"log/slog"
	"net/http"
	"os"

	"github.com/go-chi/chi/v5"
	"github.com/jackc/pgx/v5/pgxpool"
	delivery "my-service/internal/delivery/http"
	"my-service/internal/repository"
	"my-service/internal/service"
)

func main() {
	logger := slog.New(slog.NewJSONHandler(os.Stdout, nil))
	ctx := context.Background()

	pool, err := pgxpool.New(ctx, os.Getenv("DATABASE_URL"))
	if err != nil {
		logger.Error("failed to connect to db", "err", err)
		os.Exit(1)
	}
	defer pool.Close()

	// Сборка слоев (Dependency Injection снизу вверх)
	userRepo := repository.NewPgUserRepository(pool)
	userService := service.NewUserService(userRepo)
	userHandler := delivery.NewUserHandler(userService)

	r := chi.NewRouter()
	r.Post("/api/v1/users", userHandler.Register)

	logger.Info("server starting on :8080")
	if err := http.ListenAndServe(":8080", r); err != nil {
		logger.Error("server stopped", "err", err)
	}
}
```
