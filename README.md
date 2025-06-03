```
# 📚 Proofed and Polished Backend API

Proofed and Polished is a backend API built with **ASP.NET Core 8** and **PostgreSQL** that manages book tracking, user roles, and favorites for an editorial workflow. It supports role-based operations and full CRUD functionality across multiple resources.

---

## 🔧 Setup Instructions

### 1. Clone the Repo  
**Bash**
```bash
git clone https://github.com/your-username/proofed-and-polished.git
cd proofed-and-polished
```

### 2. Configure the Database  
Ensure PostgreSQL is running locally. Update your `appsettings.json` in the `src` folder to include:

**JSON**
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=ProofedDB;Username=your_user;Password=your_password"
}
```

Alternatively, use environment variables for secure configuration in production.

### 3. Run the Application  
**Bash**
```bash
cd src
dotnet run
```

Visit Swagger UI at:  
```bash
https://localhost:PORT/swagger
```

---

## 🌐 API Endpoints

### 📘 Books  
- `GET /api/books` - Get all books  
- `GET /api/books/{id}` - Get book by ID  
- `POST /api/books` - Create a new book  
- `PUT /api/books/{id}` - Update an existing book  
- `DELETE /api/books/{id}` - Delete a book  

### 👥 Users  
- `GET /api/users` - Get all users  
- `GET /api/users/{id}` - Get a user by ID  
- `POST /api/users` - Create a user  
- `PUT /api/users/{id}` - Update a user  
- `DELETE /api/users/{id}` - Delete a user  

### 🧠 Genres  
- `GET /api/genres` - Get all genres  
- `POST /api/genres` - Create a genre  
- `PUT /api/genres/{id}` - Update a genre  
- `DELETE /api/genres/{id}` - Delete a genre  

### 💬 Services  
- `GET /api/services` - Get all services  
- `POST /api/services` - Create a service  
- `PUT /api/services/{id}` - Update a service  
- `DELETE /api/services/{id}` - Delete a service  

### ⭐ Favorites  
- `GET /api/favorites` - Get all favorites (includes related Book)  
- `POST /api/favorites` - Create a favorite  
- `PUT /api/favorites/{id}` - Update a note for a favorite  
- `DELETE /api/favorites/{id}` - Delete a favorite  

---

## 🧪 Unit Testing

Unit tests are written using **xUnit** and test the main CRUD operations of:

✅ Books  
✅ Users  
✅ Genres  
✅ Services  
✅ Favorites  

### Run Tests  
**Bash**
```bash
cd tests
dotnet test
```

### Example Test Coverage  
✅ Create, update, and delete a Book  
✅ Create and update a User  
✅ Add and remove a Favorite  
✅ Validate HTTP status codes and response payloads  
✅ Use In-Memory Database for test isolation  

---

## 🧰 Tech Stack  
- ASP.NET Core 8.0  
- Entity Framework Core  
- PostgreSQL  
- Swagger for API docs  
- xUnit for testing  

---

## 🛡️ Future Improvements  
- Role-based access control via middleware  
- Auth integration using Firebase/Auth0  
- Full-text search support  
- Filtering and pagination  

---

## 🤝 Contributing  
We welcome contributions! To contribute:

1. Fork the repo  
2. Create your feature branch  
   ```bash
   git checkout -b feature/my-feature
   ```
3. Commit your changes  
   ```bash
   git commit -am 'Add feature'
   ```
4. Push to the branch  
   ```bash
   git push origin feature/my-feature
   ```
5. Open a Pull Request  

---

## Loom Video
https://www.loom.com/share/6513766b2a2e4e7cab91ac7ed157228a?sid=2a1aaa1b-3330-4805-ae31-6e9e1c9f08be

## ERD

https://drawsql.app/teams/nashville-software-school-3/diagrams/proofed-and-polished

## Postman Documentation

https://documenter.getpostman.com/view/31594183/2sB2qi6xDm

## 📄 License  
This project is open source and available under the **MIT License**.
```
