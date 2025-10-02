package com.capstone.Arogya.controller;

import com.capstone.Arogya.dto.AnswerDto;
import com.capstone.Arogya.service.AiService;
import com.capstone.Arogya.service.AuthService;
import com.capstone.Arogya.service.QuestionService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.security.SecurityRequirement;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.stream.Collectors;

@RestController
@RequestMapping("/api/ai")
@RequiredArgsConstructor
@Tag(name = "AI", description = "Endpoints for AI-driven health suggestions")
@SecurityRequirement(name = "bearerAuth")
public class AiController {

    private final AiService aiService;
    private final AuthService authService;
    private final QuestionService questionService;

    @Operation(
            summary = "Get personalized health suggestions",
            description = "Generates AI-based health suggestions based on the user's answered questions."
    )
    @ApiResponses({
            @ApiResponse(responseCode = "200", description = "Health suggestions retrieved successfully",
                    content = @Content(mediaType = "application/json",
                            schema = @Schema(implementation = String.class))),
            @ApiResponse(responseCode = "401", description = "Unauthorized", content = @Content)
    })
    @GetMapping("/health-suggestions")
    public ResponseEntity<String> getHealthSuggestions(
            @Parameter(hidden = true) Authentication authentication) {

        String username = authentication.getName();
        Long userId = authService.getUserId(username);

        List<AnswerDto> answers = questionService.getAnswersForUser(userId);

        if (answers.isEmpty()) {
            return ResponseEntity.ok("No answers yet.");
        }

        String userProfile = answers.stream()
                .map(a -> {
                    String answerText = a.getQuestionType().name().equals("MCQ")
                            ? (a.getSelectedOptionText() != null ? a.getSelectedOptionText() : "")
                            : (a.getAnswerText() != null ? a.getAnswerText()
                            : (a.getAnswerInt() != null ? a.getAnswerInt().toString() : ""));
                    return a.getQuestionId() + ": " + answerText;
                })
                .collect(Collectors.joining("; "));

        String suggestions = aiService.getHealthSuggestions(userProfile);
        return ResponseEntity.ok(suggestions);
    }
}
