using System;
using System.Collections.Generic;
using Npgsql;
using StudentManagement;

namespace ConsoleApp2
{
    class Program
    {
        static void Main()
        {
            var studentService = new StudentService();

            while (true)
            {
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1 - Показать список студентов");
                Console.WriteLine("2 - Добавить студента");
                Console.WriteLine("3 - Обновить данные студента");
                Console.WriteLine("4 - Добавить оценку студенту");
                Console.WriteLine("0 - Выход");

                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            Console.Write("Выберите сортировку (grade, name, course): ");
                            string sortBy = Console.ReadLine();
                            var students = studentService.GetStudents(sortBy);
                            foreach (var student in students)
                            {
                                Console.WriteLine($"{student.FullName}, Курс: {student.Course}, Средний балл: {student.AverageGrade}");
                            }
                            break;
                        case "2":
                            Console.Write("ФИО: ");
                            string fullName = Console.ReadLine();
                            Console.Write("Дата рождения (ГГГГ-ММ-ДД): ");
                            DateTime birthDate = DateTime.Parse(Console.ReadLine());
                            Console.Write("Год поступления: ");
                            int enrollmentYear = int.Parse(Console.ReadLine());
                            Console.Write("Курс: ");
                            int course = int.Parse(Console.ReadLine());
                            Console.Write("Группа: ");
                            string groupName = Console.ReadLine();
                            studentService.AddStudent(fullName, birthDate, enrollmentYear, course, groupName);
                            Console.WriteLine($"Студент {fullName} добавлен!");
                            break;
                        case "3":
                            Console.Write("ID студента: ");
                            int id = int.Parse(Console.ReadLine());
                            Console.Write("Новое ФИО: ");
                            string newName = Console.ReadLine();
                            Console.Write("Новый курс: ");
                            int newCourse = int.Parse(Console.ReadLine());
                            Console.Write("Новая группа: ");
                            string newGroup = Console.ReadLine();
                            studentService.UpdateStudent(id, newName, newCourse, newGroup);
                            Console.WriteLine($"Студент {newName} обновлен!");
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
    }
}


      


public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public int EnrollmentYear { get; set; }
        public int Course { get; set; }
        public string GroupName { get; set; }
        public decimal AverageGrade { get; set; }
    }

  
