package com.capstone.Arogya.controller;

import com.capstone.Arogya.dto.QuestionDto;
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

@RestController
@RequestMapping("/api/questions")
@RequiredArgsConstructor
@Tag(name = "Questions", description = "Endpoints for managing and retrieving questions")
@SecurityRequirement(name = "bearerAuth")
public class QuestionController {

    private final QuestionService questionService;
    private final AuthService authService;

    @Operation(
            summary = "Get next question for the authenticated user",
            description = "Returns the next unanswered question for the current user based on their progress."
    )
    @ApiResponses({
            @ApiResponse(responseCode = "200", description = "Next question retrieved",
                    content = @Content(mediaType = "application/json",
                            schema = @Schema(implementation = QuestionDto.class))),
            @ApiResponse(responseCode = "401", description = "Unauthorized", content = @Content),
            @ApiResponse(responseCode = "404", description = "No next question available", content = @Content)
    })
    @GetMapping("/next")
    public ResponseEntity<QuestionDto> getNextQuestion(
            @Parameter(hidden = true) Authentication authentication) {
        String username = authentication.getName();
        Long userId = authService.getUserId(username);
        QuestionDto nextQuestion = questionService.getNextQuestionForUser(userId);
        return ResponseEntity.ok(nextQuestion);
    }
}
