namespace CustomerManager.DTOs;

public record CustomerStatsDTO(
    int TotalCount,
    int ActiveCount,
    int InactiveCount,
    List<Top5CitiesDTO> Top5Cities
);
