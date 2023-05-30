using IBISWorldTest.Models.DTO;

namespace IBISWorldTest.Data
{
    public static class GlossaryStore
    {
        public static List<TermDTO> terms = new()
        {
            new TermDTO{ Id = 1, Name = "Coding", Definition = "Practice of writing program"},
            new TermDTO{ Id = 2, Name = "Ibis", Definition = "A great company to work"},
            new TermDTO{ Id = 3, Name = "Web API application", Definition = "Good way of making web apps"}
        };
    }
}
