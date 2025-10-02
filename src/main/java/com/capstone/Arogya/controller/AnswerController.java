package com.capstone.Arogya.controller;

import com.capstone.Arogya.dto.AnswerDto;
import com.capstone.Arogya.dto.SubmitAnswerDto;
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
@RequestMapping("/api/answers")
@RequiredArgsConstructor
@Tag(name = "Answers", description = "Endpoints for submitting and managing answers")
@SecurityRequirement(name = "bearerAuth")
public class AnswerController {

    private final QuestionService questionService;
    private final AuthService authService;

    @Operation(
            summary = "Submit an answer for the current user",
            description = "Takes a submitted answer and associates it with the authenticated user."
    )
    @ApiResponses({
            @ApiResponse(responseCode = "200", description = "Answer submitted successfully",
                    content = @Content(mediaType = "application/json",
                            schema = @Schema(implementation = AnswerDto.class))),
            @ApiResponse(responseCode = "400", description = "Invalid request payload", content = @Content),
            @ApiResponse(responseCode = "401", description = "Unauthorized", content = @Content)
    })
    @PostMapping
    public ResponseEntity<AnswerDto> submitAnswer(
            @RequestBody SubmitAnswerDto dto,
            @Parameter(hidden = true) Authentication authentication) {
        String username = authentication.getName();
        Long userId = authService.getUserId(username);
        AnswerDto saved = questionService.submitAnswer(dto, userId);
        return ResponseEntity.ok(saved);
    }
}
