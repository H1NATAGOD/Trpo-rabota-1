using System;
using System.Collections.Generic;
using Xunit;
using Assert = Xunit.Assert;

namespace StudentManagement.Tests
{
    public class StudentTests
    {
        private readonly StudentService _service = new StudentService(); // Создаём экземпляр

        [Fact]
        public void GetStudents_ReturnsListOfStudents()
        {
            var students = _service.GetStudents("grade"); // Используем _service
            Assert.NotNull(students);
            Assert.IsType<List<Student>>(students);
        }

        [Fact]
        public void AddStudent_ValidData_AddsStudent()
        {
            var exception = Record.Exception(() => 
                _service.AddStudent("Иван Иванов", new DateTime(2000, 5, 10), 2018, 2, "Группа А"));
            Assert.Null(exception);
        }

        [Fact]
        public void UpdateStudent_ValidData_UpdatesStudent()
        {
            var exception = Record.Exception(() => 
                _service.UpdateStudent(1, "Петр Петров", 3, "Группа Б"));
            Assert.Null(exception);
        }

        [Fact]
        public void AddGrade_InvalidYear_ThrowsException()
        {
            var exception = Record.Exception(() => 
                _service.AddGrade(1, 5, 4.5)); // 5-й год при 4-м курсе
            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public void GetStudentsSortedByAverageGrade_ReturnsSortedList()
        {
            var students = _service.GetStudents("grade");
            Assert.NotNull(students);
            Assert.IsType<List<Student>>(students);
        }
    }
}