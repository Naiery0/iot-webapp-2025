DELIMITER $$

CREATE PROCEDURE `Board_PagingBoard`(
    IN startCount INT,
    IN endCount INT,
    IN search VARCHAR(100)
)
BEGIN
    SELECT * 
    FROM (
        SELECT ROW_NUMBER() OVER (ORDER BY PostDate DESC) AS rowNum,
               Email, Id, Writer, Title, Contents, PostDate, ReadCount
        FROM News
        WHERE Title LIKE CONCAT('%', search, '%')
    ) AS b
    WHERE b.rowNum BETWEEN startCount AND endCount;
END$$

DELIMITER ;