using System;
using System.Collections.Generic;
using Npgsql;

class Program
{
    private const string ConnectionString = "Host=localhost;Username=postgres;Password=1234;Database=postgres";

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1 - Показать список студентов");
            Console.WriteLine("2 - Добавить студента");
            Console.WriteLine("3 - Обновить данные студента");
            Console.WriteLine("4- Добавить оценку студенту");
            Console.WriteLine("0 - Выход");
            
            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        Console.Write("Выберите сортировку (grade, name, course): ");
                        string sortBy = Console.ReadLine();
                        var students = GetStudents(sortBy);
                        foreach (var student in students)
                        {
                            Console.WriteLine($"{student.FullName}, Курс: {student.Course}, Средний балл: {student.AverageGrade}");
                        }
                        break;
                    case "2":
                        AddStudent();
                        break;
                    case "3":
                        UpdateStudent();
                        break;
                    case "4":
                        AddGrade();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор, попробуйте снова.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    static void AddGrade()
    {
        Console.Write("ID студента: ");
        if (!int.TryParse(Console.ReadLine(), out int studentId)) return;
        Console.Write("Год оценки: ");
        if (!int.TryParse(Console.ReadLine(), out int year)) return;
        Console.Write("Оценка: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal grade)) return;
        
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        
        string checkQuery = "SELECT course FROM students WHERE id = @id";
        using var checkCmd = new NpgsqlCommand(checkQuery, conn);
        checkCmd.Parameters.AddWithValue("id", studentId);
        object result = checkCmd.ExecuteScalar();
        
        if (result == null)
        {
            Console.WriteLine("Студент не найден.");
            return;
        }
        int maxYear = Convert.ToInt32(result);
        if (year > maxYear)
        {
            Console.WriteLine("Ошибка: Год оценки не может превышать текущий курс студента.");
            return;
        }
        
        string query = "INSERT INTO grades (student_id, year, grade) VALUES (@studentId, @year, @grade)";
        using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("studentId", studentId);
        cmd.Parameters.AddWithValue("year", year);
        cmd.Parameters.AddWithValue("grade", grade);
        cmd.ExecuteNonQuery();
        
        Console.WriteLine("Оценка добавлена!");
    }

    static List<Student> GetStudents(string sortBy = "grade")
    {
        List<Student> students = new List<Student>();
        string orderBy = sortBy switch
        {
            "name" => "s.full_name",
            "course" => "s.course",
            _ => "average_grade DESC"
        };
        
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        string query = $@"
            SELECT s.id, s.full_name, s.birth_date, s.enrollment_year, s.course, s.group_name,
                   COALESCE(AVG(g.grade), 0) AS average_grade
            FROM students s
            LEFT JOIN grades g ON s.id = g.student_id
            GROUP BY s.id
            ORDER BY {orderBy}";

        using var cmd = new NpgsqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            students.Add(new Student
            {
                Id = reader.GetInt32(0),
                FullName = reader.GetString(1),
                BirthDate = reader.GetDateTime(2),
                EnrollmentYear = reader.GetInt32(3),
                Course = reader.GetInt32(4),
                GroupName = reader.GetString(5),
                AverageGrade = reader.GetDecimal(6)
            });
        }
        return students;
    }

    static void AddStudent()
    {
        Console.Write("ФИО: ");
        string fullName = Console.ReadLine();
        Console.Write("Дата рождения (ГГГГ-ММ-ДД): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime birthDate)) return;
        Console.Write("Год поступления: ");
        if (!int.TryParse(Console.ReadLine(), out int enrollmentYear)) return;
        Console.Write("Курс: ");
        if (!int.TryParse(Console.ReadLine(), out int course)) return;
        Console.Write("Группа: ");
        string groupName = Console.ReadLine();
        
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        string query = @"INSERT INTO students (full_name, birth_date, enrollment_year, course, group_name)
                         VALUES (@name, @birth, @enrollment, @course, @group)";
        using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("name", fullName);
        cmd.Parameters.AddWithValue("birth", birthDate);
        cmd.Parameters.AddWithValue("enrollment", enrollmentYear);
        cmd.Parameters.AddWithValue("course", course);
        cmd.Parameters.AddWithValue("group", groupName);
        cmd.ExecuteNonQuery();
        Console.WriteLine($"Студент {fullName} добавлен!");
    }

    static void UpdateStudent()
    {
        Console.Write("ID студента: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) return;
        Console.Write("Новое ФИО: ");
        string fullName = Console.ReadLine();
        Console.Write("Новый курс: ");
        if (!int.TryParse(Console.ReadLine(), out int course)) return;
        Console.Write("Новая группа: ");
        string groupName = Console.ReadLine();
        
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        string query = @"UPDATE students 
                         SET full_name = @name, course = @course, group_name = @group
                         WHERE id = @id";
        using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("name", fullName);
        cmd.Parameters.AddWithValue("course", course);
        cmd.Parameters.AddWithValue("group", groupName);
        cmd.ExecuteNonQuery();
        Console.WriteLine($"Студент {fullName} обновлен!");
    }
}

class Student
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public DateTime BirthDate { get; set; }
    public int EnrollmentYear { get; set; }
    public int Course { get; set; }
    public string GroupName { get; set; }
    public decimal AverageGrade { get; set; }
}
  
