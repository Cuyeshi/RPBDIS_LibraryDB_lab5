-- ”даление существующего ограничени€ внешнего ключа
ALTER TABLE LoanedBooks
DROP CONSTRAINT FK__LoanedBoo__BookI__4222D4EF;

-- ƒобавление нового ограничени€ с каскадным удалением
ALTER TABLE LoanedBooks
ADD CONSTRAINT FK__LoanedBoo__BookI__4222D4EF
FOREIGN KEY (BookID)
REFERENCES Books(BookId)
ON DELETE CASCADE;
