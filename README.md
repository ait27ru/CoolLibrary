# CoolLibrary
A simple library system.

# Usage
1. Clone the repository to your local drive and open the solution in Visual Studio 2022.
2. Build the solution.
3. In the Package Manager Console, run `update-database`. This will create users, genres, and books in the system.
4. Run the solution without debugging (Ctrl-F5).
5. Log in as `joe.bloggs@gmail.com` with the password: `Test1234$`.
6. Use the UI to add books to the shopping cart, then follow the process to complete checkout. This will create a borrowing record with an empty return date. The borrowing record is only visible to its owner and is read-only.
7. Log in as `john.smith@gmail.com` with the password: `Test1234$`.
8. Repeat the process to borrow books.
9. Log in as `admin@gmail.com` with the password: `Test1234$`.
10. Open the borrowings menu to view the borrowings made by other users. Use the UI to edit the return date of a borrowing.
11. Open Inventory -> Genre and Inventory -> Books to add genres and books.

# Admin view
![](/Library.png)
