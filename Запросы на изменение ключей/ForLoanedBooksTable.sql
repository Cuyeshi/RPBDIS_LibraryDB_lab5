-- �������� ������������� ����������� �������� �����
ALTER TABLE LoanedBooks
DROP CONSTRAINT FK__LoanedBoo__BookI__4222D4EF;

-- ���������� ������ ����������� � ��������� ���������
ALTER TABLE LoanedBooks
ADD CONSTRAINT FK__LoanedBoo__BookI__4222D4EF
FOREIGN KEY (BookID)
REFERENCES Books(BookId)
ON DELETE CASCADE;
